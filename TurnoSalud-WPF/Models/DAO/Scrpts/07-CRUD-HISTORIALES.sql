

CREATE TABLE HISTORIALES (
    IdHistorial     INT IDENTITY(1,1) PRIMARY KEY,
    IdTurno         INT NOT NULL UNIQUE,
    Sintomas        NVARCHAR(MAX), 
    Diagnostico     NVARCHAR(MAX),
    Tratamiento     NVARCHAR(MAX),
    Observaciones   NVARCHAR(MAX),
    FechaRegistro   DATETIME NOT NULL DEFAULT GETDATE(),
    
    FOREIGN KEY (IdTurno) REFERENCES TURNOS(IdTurno) ON DELETE CASCADE
);
GO


CREATE PROCEDURE sp_RegistrarHistorial
    @idTurno        INT,
    @sintomas       NVARCHAR(MAX),
    @diagnostico    NVARCHAR(MAX),
    @tratamiento    NVARCHAR(MAX),
    @observaciones  NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
   
    IF EXISTS (SELECT 1 FROM HISTORIALES WHERE IdTurno = @idTurno)
    BEGIN
        RAISERROR('Ya existe un registro de consulta para este turno.', 16, 1);
        RETURN;
    END

    DECLARE @idEstadoAtendido INT;
    SELECT @idEstadoAtendido = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Atendido';
    IF @idEstadoAtendido IS NULL
    BEGIN
        RAISERROR('El estado "Atendido" no se encuentra en la tabla ESTADOS_TURNO.', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO HISTORIALES (IdTurno, Sintomas, Diagnostico, Tratamiento, Observaciones)
        VALUES (@idTurno, @sintomas, @diagnostico, @tratamiento, @observaciones);

        UPDATE TURNOS
        SET IdEstado = @idEstadoAtendido
        WHERE IdTurno = @idTurno;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE PROCEDURE sp_ListarHistorialPorPaciente
    @idPaciente INT
AS
BEGIN
    SELECT
        H.IdHistorial,
        T.IdTurno,
        T.FechaTurno,
        E.Nombre AS Especialidad,
        U.Nombre + ' ' + U.Apellido AS Medico,
        H.Sintomas,
        H.Diagnostico,
        H.Tratamiento,
        H.Observaciones,
        H.FechaRegistro
    FROM HISTORIALES H
    INNER JOIN TURNOS T ON H.IdTurno = T.IdTurno
    INNER JOIN MEDICOS M ON T.IdMedico = M.IdMedico
    INNER JOIN USUARIOS U ON M.IdUsuario = U.IdUsuario
    INNER JOIN ESPECIALIDADES E ON M.IdEspecialidad = E.IdEspecialidad
    WHERE T.IdPaciente = @idPaciente
    ORDER BY H.FechaRegistro DESC;
END
GO

CREATE PROCEDURE sp_ObtenerHistorialPorTurno
    @idTurno INT
AS
BEGIN
    SELECT IdHistorial, IdTurno, Sintomas, Diagnostico, Tratamiento, Observaciones, FechaRegistro
    FROM HISTORIALES
    WHERE IdTurno = @idTurno;
END
GO

CREATE PROCEDURE sp_ListarPacientesAtendidosPorMedico
    @idMedico INT,
    @filtro NVARCHAR(100) = NULL
AS
BEGIN
    SELECT DISTINCT
        P.IdPaciente,
        P.DNI,
        P.Nombre,
        P.Apellido
    FROM HISTORIALES H
    INNER JOIN TURNOS T ON H.IdTurno = T.IdTurno
    INNER JOIN PACIENTES P ON T.IdPaciente = P.IdPaciente
    WHERE T.IdMedico = @idMedico
      AND (
          @filtro IS NULL OR @filtro = ''
          OR P.DNI LIKE '%' + @filtro + '%'
          OR P.Nombre LIKE '%' + @filtro + '%'
          OR P.Apellido LIKE '%' + @filtro + '%'
      );
END
GO