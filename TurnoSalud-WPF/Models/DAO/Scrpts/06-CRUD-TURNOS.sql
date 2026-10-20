CREATE TABLE ESTADOS_TURNO (
    IdEstado INT PRIMARY KEY,
    Nombre VARCHAR(20) NOT NULL UNIQUE
);
GO

CREATE TABLE TURNOS (
    IdTurno INT PRIMARY KEY IDENTITY(1,1),
    IdPaciente INT NOT NULL,
    IdMedico INT NOT NULL,
    IdFranja INT NOT NULL,
    FechaTurno DATE NOT NULL, 
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    IdEstado INT NOT NULL,

    FOREIGN KEY (IdPaciente) REFERENCES PACIENTES(IdPaciente),
    FOREIGN KEY (IdMedico) REFERENCES MEDICOS(IdMedico),
    FOREIGN KEY (IdFranja) REFERENCES FRANJA_HORARIO(IdFranja),
    FOREIGN KEY (IdEstado) REFERENCES ESTADOS_TURNO(IdEstado)
);
GO


INSERT INTO ESTADOS_TURNO (IdEstado, Nombre) VALUES
(1, 'Pendiente'),
(2, 'Cancelado'),
(3, 'Atendido'),
(4, 'Vencido');

GO


ALTER PROCEDURE sp_RegistrarTurno
    @idPaciente INT,
    @idMedico INT,
    @idFranja INT,
    @fecha DATE
AS
BEGIN
    SET DATEFIRST 1; 
    DECLARE @diaSemana INT = DATEPART(WEEKDAY, @fecha);
    DECLARE @idEstadoPendiente INT;
    SELECT @idEstadoPendiente = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Pendiente';

    IF NOT EXISTS (SELECT 1 FROM HORARIOS_MEDICOS WHERE IdMedico = @idMedico AND IdFranja = @idFranja AND DiaSemana = @diaSemana AND Estado = 1)
    BEGIN
        RAISERROR('El médico no tiene disponible ese horario en el día seleccionado.', 16, 1); RETURN;
    END

    IF EXISTS (
        SELECT 1 FROM TURNOS
        WHERE IdMedico = @idMedico
          AND IdFranja = @idFranja
          AND FechaTurno = @fecha
          AND IdEstado = @idEstadoPendiente 
    )
    BEGIN
        RAISERROR('El turno ya se encuentra ocupado.', 16, 1); RETURN;
    END

    INSERT INTO TURNOS (IdPaciente, IdMedico, IdFranja, FechaTurno, IdEstado)
    VALUES (@idPaciente, @idMedico, @idFranja, @fecha, @idEstadoPendiente);

    SELECT SCOPE_IDENTITY() AS IdTurno;
END
GO

CREATE PROCEDURE sp_ListarTurnosPorMedico
    @idMedico INT,
    @fecha DATE,
    @idEstado INT 
AS
BEGIN
    SELECT T.IdTurno, P.Nombre + ' ' + P.Apellido AS Paciente,
           BH.Nombre AS Bloque, FH.HoraInicio, FH.HoraFin,
           T.FechaTurno, T.FechaRegistro, ET.Nombre AS Estado
    FROM TURNOS T
    INNER JOIN PACIENTES P ON T.IdPaciente = P.IdPaciente
    INNER JOIN FRANJA_HORARIO FH ON T.IdFranja = FH.IdFranja
    INNER JOIN BLOQUE_HORARIO BH ON FH.IdBloque = BH.IdBloque
    INNER JOIN ESTADOS_TURNO ET ON T.IdEstado = ET.IdEstado
    WHERE T.IdMedico = @idMedico AND T.FechaTurno = @fecha AND T.IdEstado = @idEstado
    ORDER BY FH.HoraInicio;
END
GO

CREATE PROCEDURE sp_ListarTurnosPorPaciente
    @idPaciente INT,
    @idEstado INT = NULL, 
    @mostrarPasados BIT = 0
AS
BEGIN
    SET LANGUAGE Spanish;
    SELECT T.IdTurno, T.IdFranja, DATENAME(WEEKDAY, T.FechaTurno) AS DiaNombre,
           U.Nombre + ' ' + U.Apellido AS Medico, E.Nombre AS Especialidad,
           BH.Nombre AS Bloque, FH.HoraInicio, FH.HoraFin,
           T.FechaTurno, T.FechaRegistro, ET.Nombre AS Estado
    FROM TURNOS T
    INNER JOIN MEDICOS M ON T.IdMedico = M.IdMedico
    INNER JOIN USUARIOS U ON M.IdUsuario = U.IdUsuario
    INNER JOIN ESPECIALIDADES E ON M.IdEspecialidad = E.IdEspecialidad
    INNER JOIN FRANJA_HORARIO FH ON T.IdFranja = FH.IdFranja
    INNER JOIN BLOQUE_HORARIO BH ON FH.IdBloque = BH.IdBloque
    INNER JOIN ESTADOS_TURNO ET ON T.IdEstado = ET.IdEstado
    WHERE T.IdPaciente = @idPaciente
      AND (@idEstado IS NULL OR T.IdEstado = @idEstado)
      AND (@mostrarPasados = 1 OR T.FechaTurno >= CAST(GETDATE() AS DATE))
    ORDER BY T.FechaTurno, FH.HoraInicio;
END
GO

CREATE PROCEDURE sp_CambiarEstadoTurno
    @idTurno INT,
    @nuevoEstadoNombre VARCHAR(20) 
AS
BEGIN
    DECLARE @idNuevoEstado INT;
    SELECT @idNuevoEstado = IdEstado FROM ESTADOS_TURNO WHERE Nombre = @nuevoEstadoNombre;

    IF @idNuevoEstado IS NULL
    BEGIN
        RAISERROR('El estado especificado no es válido.', 16, 1); RETURN;
    END

    UPDATE TURNOS SET IdEstado = @idNuevoEstado WHERE IdTurno = @idTurno;
END
GO

CREATE PROCEDURE sp_ModificarTurno
    @idTurno INT,
    @idMedico INT,
    @idFranja INT,
    @fecha DATE
AS
BEGIN
    SET DATEFIRST 1;
    DECLARE @diaSemana INT = DATEPART(WEEKDAY, @fecha);

    IF NOT EXISTS (SELECT 1 FROM HORARIOS_MEDICOS WHERE IdMedico = @idMedico AND IdFranja = @idFranja AND DiaSemana = @diaSemana AND Estado = 1)
    BEGIN
        RAISERROR('El médico no tiene disponible ese horario en el día seleccionado.', 16, 1); RETURN;
    END

    IF EXISTS (SELECT 1 FROM TURNOS WHERE IdMedico = @idMedico AND IdFranja = @idFranja AND FechaTurno = @fecha AND IdEstado IN (1, 2) AND IdTurno <> @idTurno)
    BEGIN
        RAISERROR('El nuevo horario ya se encuentra ocupado.', 16, 1); RETURN;
    END

    UPDATE TURNOS SET IdMedico = @idMedico, IdFranja = @idFranja, FechaTurno = @fecha
    WHERE IdTurno = @idTurno;
END
GO

ALTER PROCEDURE sp_ListarTurnosDelDia
AS
BEGIN
    SET LANGUAGE Spanish;
    DECLARE @idEstadoPendiente INT;
    SELECT @idEstadoPendiente = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Pendiente';

    SELECT
        T.IdTurno, P.DNI, P.Nombre + ' ' + P.Apellido AS Paciente,
        BH.Nombre AS Bloque, FH.HoraInicio, FH.HoraFin, T.FechaTurno, ET.Nombre AS Estado
    FROM TURNOS T
    INNER JOIN PACIENTES P ON T.IdPaciente = P.IdPaciente
    INNER JOIN FRANJA_HORARIO FH ON T.IdFranja = FH.IdFranja
    INNER JOIN BLOQUE_HORARIO BH ON FH.IdBloque = BH.IdBloque
    INNER JOIN ESTADOS_TURNO ET ON T.IdEstado = ET.IdEstado
    WHERE T.FechaTurno = CAST(GETDATE() AS DATE) 
      AND T.IdEstado = @idEstadoPendiente
    ORDER BY FH.HoraInicio;
END
GO
CREATE PROCEDURE sp_MarcarTurnosVencidos
AS
BEGIN
    SET NOCOUNT ON; 

    DECLARE @idEstadoPendiente INT;
    DECLARE @idEstadoVencido INT;

    SELECT @idEstadoPendiente = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Pendiente';
    SELECT @idEstadoVencido = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Vencido';

    UPDATE T
    SET 
        T.IdEstado = @idEstadoVencido
    FROM 
        TURNOS AS T
    WHERE 
        T.FechaTurno < CAST(GETDATE() AS DATE)
        
        AND T.IdEstado = @idEstadoPendiente
        
        
        AND NOT EXISTS (
            SELECT 1 
            FROM HISTORIALES H 
            WHERE H.IdTurno = T.IdTurno
        );
END
GO


