

CREATE TABLE BLOQUE_HORARIO (
    IdBloque INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE FRANJA_HORARIO (
    IdFranja INT PRIMARY KEY IDENTITY(1,1),
    IdBloque INT NOT NULL,
    HoraInicio TIME(0) NOT NULL, 
    HoraFin TIME(0) NOT NULL,    
    FOREIGN KEY (IdBloque) REFERENCES BLOQUE_HORARIO(IdBloque)
);
GO

CREATE TABLE HORARIOS_MEDICOS (
    IdMedico INT NOT NULL,
    IdFranja INT NOT NULL,
    DiaSemana INT NOT NULL CHECK (DiaSemana BETWEEN 1 AND 7), 
    Estado BIT NOT NULL DEFAULT 1,
    PRIMARY KEY (IdMedico, IdFranja, DiaSemana), 
    FOREIGN KEY (IdMedico) REFERENCES MEDICOS(IdMedico) ON DELETE CASCADE,
    FOREIGN KEY (IdFranja) REFERENCES FRANJA_HORARIO(IdFranja)
);
GO


INSERT INTO BLOQUE_HORARIO (Nombre) VALUES ('Mañana'), ('Tarde'), ('Noche');
GO

INSERT INTO FRANJA_HORARIO (IdBloque, HoraInicio, HoraFin) VALUES
(1, '09:00:00', '09:30:00'), (1, '09:30:00', '10:00:00'), (1, '10:00:00', '10:30:00'),
(1, '10:30:00', '11:00:00'), (1, '11:00:00', '11:30:00'), (1, '11:30:00', '12:00:00');

INSERT INTO FRANJA_HORARIO (IdBloque, HoraInicio, HoraFin) VALUES
(2, '14:00:00', '14:30:00'), (2, '14:30:00', '15:00:00'), (2, '15:00:00', '15:30:00'),
(2, '15:30:00', '16:00:00'), (2, '16:00:00', '16:30:00'), (2, '16:30:00', '17:00:00');

INSERT INTO FRANJA_HORARIO (IdBloque, HoraInicio, HoraFin) VALUES
(3, '20:00:00', '20:30:00'), (3, '20:30:00', '21:00:00'), (3, '21:00:00', '21:30:00'),
(3, '21:30:00', '22:00:00'), (3, '22:00:00', '22:30:00'), (3, '22:30:00', '23:00:00');
GO


IF TYPE_ID('dbo.FranjaAsignacion') IS NOT NULL DROP TYPE dbo.FranjaAsignacion;
GO
CREATE TYPE FranjaAsignacion AS TABLE (
    IdFranja INT,
    DiaSemana INT
);
GO



CREATE PROCEDURE sp_ListarBloquesHorario
AS BEGIN SELECT IdBloque, Nombre FROM BLOQUE_HORARIO; END
GO
CREATE PROCEDURE sp_ListarFranjasPorBloque @idBloque INT
AS BEGIN SELECT IdFranja, HoraInicio, HoraFin FROM FRANJA_HORARIO WHERE IdBloque = @idBloque; END
GO

CREATE PROCEDURE sp_AsignarHorariosMedico
    @idMedico INT,
    @asignaciones FranjaAsignacion READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE HM SET Estado = 1
        FROM HORARIOS_MEDICOS HM
        INNER JOIN @asignaciones A ON HM.IdFranja = A.IdFranja AND HM.DiaSemana = A.DiaSemana
        WHERE HM.IdMedico = @idMedico AND HM.Estado = 0;

        INSERT INTO HORARIOS_MEDICOS (IdMedico, IdFranja, DiaSemana, Estado)
        SELECT @idMedico, A.IdFranja, A.DiaSemana, 1
        FROM @asignaciones A
        WHERE NOT EXISTS (
            SELECT 1 FROM HORARIOS_MEDICOS HM
            WHERE HM.IdMedico = @idMedico 
              AND HM.IdFranja = A.IdFranja 
              AND HM.DiaSemana = A.DiaSemana
        );
        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW; 
    END CATCH
END
GO

ALTER PROCEDURE sp_DarBajaHorarioMedico
    @idMedico INT,
    @idFranja INT,
    @diaSemana INT
AS
BEGIN
    SET NOCOUNT ON;
    SET DATEFIRST 1; 

    DECLARE @idEstadoPendiente INT;
    
    SELECT @idEstadoPendiente = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Pendiente';

    IF EXISTS (
        SELECT 1 FROM TURNOS
        WHERE IdMedico = @idMedico
          AND IdFranja = @idFranja
          AND DATEPART(WEEKDAY, FechaTurno) = @diaSemana
          AND IdEstado = @idEstadoPendiente
          AND FechaTurno >= CAST(GETDATE() AS DATE)
    )
    BEGIN
        RAISERROR('No se puede eliminar el horario. Tiene turnos futuros pendientes.', 16, 1);
        RETURN;
    END

    UPDATE HORARIOS_MEDICOS SET Estado = 0
    WHERE IdMedico = @idMedico AND IdFranja = @idFranja AND DiaSemana = @diaSemana;
END
GO

CREATE PROCEDURE sp_ListarHorariosDeMedico
    @idMedico INT
AS
BEGIN
    SELECT 
        MH.DiaSemana,
        BH.Nombre AS Bloque,
        FH.IdFranja,
        FH.HoraInicio,
        FH.HoraFin
    FROM HORARIOS_MEDICOS MH
    INNER JOIN FRANJA_HORARIO FH ON MH.IdFranja = FH.IdFranja
    INNER JOIN BLOQUE_HORARIO BH ON FH.IdBloque = BH.IdBloque
    WHERE MH.IdMedico = @idMedico AND MH.Estado = 1
    ORDER BY MH.DiaSemana, FH.HoraInicio;
END
GO

CREATE PROCEDURE sp_ListarMedicosConHorarios
AS
BEGIN
    SELECT DISTINCT M.IdMedico, U.Nombre + ' ' + U.Apellido AS NombreCompleto
    FROM HORARIOS_MEDICOS H
    INNER JOIN MEDICOS M ON H.IdMedico = M.IdMedico
    INNER JOIN USUARIOS U ON M.IdUsuario = U.IdUsuario
    WHERE H.Estado = 1 AND M.Estado = 1;
END
GO



IF OBJECT_ID('dbo.fn_CalcularProximaFecha', 'FN') IS NOT NULL DROP FUNCTION dbo.fn_CalcularProximaFecha;
GO
CREATE FUNCTION fn_CalcularProximaFecha(@diaSemana INT) RETURNS DATE
AS
BEGIN
    RETURN DATEADD(wk, DATEDIFF(wk, 7, GETDATE()), @diaSemana - 1);
END
GO

ALTER PROCEDURE sp_ListarHorariosDisponiblesDelMedico
    @idMedico INT,
    @fechaReferencia DATE
AS
BEGIN
    SET DATEFIRST 1;
    DECLARE @diaSemanaRef INT = DATEPART(WEEKDAY, @fechaReferencia);
    DECLARE @idEstadoPendiente INT;
    SELECT @idEstadoPendiente = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Pendiente';

    SELECT 
        HM.DiaSemana, BH.Nombre AS Bloque, FH.IdFranja, FH.HoraInicio, FH.HoraFin
    FROM HORARIOS_MEDICOS HM
    INNER JOIN FRANJA_HORARIO FH ON HM.IdFranja = FH.IdFranja
    INNER JOIN BLOQUE_HORARIO BH ON FH.IdBloque = BH.IdBloque
    WHERE 
        HM.IdMedico = @idMedico AND HM.Estado = 1 AND HM.DiaSemana = @diaSemanaRef
        AND NOT EXISTS (
            SELECT 1 
            FROM TURNOS T
            WHERE T.IdMedico = HM.IdMedico
              AND T.IdFranja = HM.IdFranja
              AND T.FechaTurno = @fechaReferencia
              AND T.IdEstado = @idEstadoPendiente
        )
    ORDER BY FH.HoraInicio;
END
GO