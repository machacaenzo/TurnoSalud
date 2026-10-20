CREATE TABLE MEDICOS (
    IdMedico INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NOT NULL UNIQUE,
    IdEspecialidad INT NOT NULL,
    Matricula VARCHAR(50) NOT NULL,
    Estado BIT NOT NULL DEFAULT 1,
    
    FOREIGN KEY (IdUsuario) REFERENCES USUARIOS(IdUsuario),
    FOREIGN KEY (IdEspecialidad) REFERENCES ESPECIALIDADES(IdEspecialidad)
);
GO


CREATE PROCEDURE sp_AgregarMedico
    @dni VARCHAR(15), @nombre VARCHAR(100), @apellido VARCHAR(100),
    @email VARCHAR(100), @telefono VARCHAR(30), @direccion VARCHAR(200),
    @fechaNacimiento DATE, @nombreUsuario VARCHAR(50), @contrasena VARCHAR(100),
    @foto VARBINARY(MAX), @matricula VARCHAR(50), @idEspecialidad INT
AS
BEGIN
    DECLARE @idRolMedico INT;
    SELECT @idRolMedico = IdRol FROM ROLES WHERE Nombre = 'Médico';

    IF @idRolMedico IS NULL
    BEGIN
        RAISERROR('El rol "Médico" no se encuentra en la tabla ROLES.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM USUARIOS WHERE Dni = @dni AND Estado = 1)
    BEGIN
        RAISERROR('Ya existe un usuario activo con ese DNI.', 16, 1);
        RETURN;
    END
    IF EXISTS (SELECT 1 FROM USUARIOS WHERE NombreUsuario = @nombreUsuario AND Estado = 1)
    BEGIN
        RAISERROR('Ya existe un usuario activo con ese nombre de usuario.', 16, 1);
        RETURN;
    END
    IF EXISTS (SELECT 1 FROM MEDICOS WHERE Matricula = @matricula AND Estado = 1)
    BEGIN
        RAISERROR('Ya existe un médico activo con esa matrícula.', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO USUARIOS (Dni, Nombre, Apellido, Email, Telefono, Direccion, FechaNacimiento, NombreUsuario, Contrasena, IdRol, Foto)
        VALUES (@dni, @nombre, @apellido, @email, @telefono, @direccion, @fechaNacimiento, @nombreUsuario, @contrasena, @idRolMedico, @foto);

        DECLARE @idUsuario INT = SCOPE_IDENTITY();

        INSERT INTO MEDICOS (IdUsuario, Matricula, IdEspecialidad)
        VALUES (@idUsuario, @matricula, @idEspecialidad);

        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END;
GO


CREATE PROCEDURE sp_ActualizarMedico
    @idUsuario INT, @idMedico INT,
    @dni VARCHAR(15), @nombre VARCHAR(100), @apellido VARCHAR(100),
    @email VARCHAR(100), @telefono VARCHAR(30), @direccion VARCHAR(200),
    @fechaNacimiento DATE, @nombreUsuario VARCHAR(50), 
    @foto VARBINARY(MAX), @matricula VARCHAR(50), @idEspecialidad INT,
    @contrasena VARCHAR(100) = NULL 
AS
BEGIN
    IF EXISTS (SELECT 1 FROM USUARIOS WHERE Dni = @dni AND IdUsuario <> @idUsuario) BEGIN RAISERROR('Ya existe otro usuario con ese DNI.', 16, 1); RETURN; END
    IF EXISTS (SELECT 1 FROM USUARIOS WHERE NombreUsuario = @nombreUsuario AND IdUsuario <> @idUsuario) BEGIN RAISERROR('Ya existe otro usuario con ese nombre de usuario.', 16, 1); RETURN; END
    IF EXISTS (SELECT 1 FROM MEDICOS WHERE Matricula = @matricula AND IdMedico <> @idMedico AND Estado = 1) BEGIN RAISERROR('Ya existe otro médico activo con esa matrícula.', 16, 1); RETURN; END

    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE USUARIOS SET Dni = @dni, Nombre = @nombre, Apellido = @apellido, Email = @email, Telefono = @telefono,
                            Direccion = @direccion, FechaNacimiento = @fechaNacimiento, NombreUsuario = @nombreUsuario, Foto = @foto
        WHERE IdUsuario = @idUsuario;

        -- Actualizar contraseña solo si se proporciona
        IF @contrasena IS NOT NULL AND LTRIM(RTRIM(@contrasena)) <> ''
        BEGIN
            UPDATE USUARIOS SET Contrasena = @contrasena WHERE IdUsuario = @idUsuario;
        END

        -- Actualizar datos del médico
        UPDATE MEDICOS SET Matricula = @matricula, IdEspecialidad = @idEspecialidad WHERE IdMedico = @idMedico;

        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END;
GO


ALTER PROCEDURE sp_DarBajaMedico
    @idMedico INT
AS
BEGIN
    DECLARE @idEstadoPendiente INT;
    DECLARE @idUsuario INT;

    SELECT @idEstadoPendiente = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Pendiente';

    IF EXISTS (
        SELECT 1 FROM TURNOS
        WHERE IdMedico = @idMedico
          AND IdEstado = @idEstadoPendiente
          AND FechaTurno >= CAST(GETDATE() AS DATE)
    )
    BEGIN
        RAISERROR('No se puede dar de baja al médico. Tiene turnos futuros pendientes.', 16, 1);
        RETURN;
    END

    SELECT @idUsuario = IdUsuario FROM MEDICOS WHERE IdMedico = @idMedico;

    UPDATE MEDICOS SET Estado = 0 WHERE IdMedico = @idMedico;
    UPDATE USUARIOS SET Estado = 0 WHERE IdUsuario = @idUsuario;
END
GO


CREATE PROCEDURE sp_ReactivarMedico
    @idMedico INT
AS
BEGIN
    DECLARE @idUsuario INT;
    SELECT @idUsuario = IdUsuario FROM MEDICOS WHERE IdMedico = @idMedico;

    UPDATE MEDICOS SET Estado = 1 WHERE IdMedico = @idMedico;
    UPDATE USUARIOS SET Estado = 1 WHERE IdUsuario = @idUsuario;
END
GO


CREATE PROCEDURE sp_ListarMedicos
    @buscar VARCHAR(100) = '',
    @Estado BIT = 1
AS
BEGIN
    SELECT 
        M.IdMedico, M.Matricula, M.Estado, U.IdUsuario, U.Nombre, U.Apellido,
        U.Email, U.Telefono, U.Dni, U.Direccion, U.FechaNacimiento,
        U.NombreUsuario, U.Foto, E.Nombre AS Especialidad, E.IdEspecialidad
    FROM MEDICOS M
    INNER JOIN USUARIOS U ON M.IdUsuario = U.IdUsuario
    INNER JOIN ESPECIALIDADES E ON M.IdEspecialidad = E.IdEspecialidad
    WHERE M.Estado = @Estado
      AND (U.Nombre LIKE '%' + @buscar + '%' OR U.Apellido LIKE '%' + @buscar + '%' OR
           U.Dni LIKE '%' + @buscar + '%' OR M.Matricula LIKE '%' + @buscar + '%' OR
           E.Nombre LIKE '%' + @buscar + '%');
END
GO


CREATE PROCEDURE sp_ListarMedicosPorEspecialidad
    @idEspecialidad INT
AS
BEGIN
    SELECT 
        U.Foto, U.Nombre, U.Apellido, M.IdMedico
    FROM MEDICOS M
    INNER JOIN USUARIOS U ON M.IdUsuario = U.IdUsuario
    WHERE M.Estado = 1 
      AND M.IdEspecialidad = @idEspecialidad
      AND (OBJECT_ID('HORARIOS_MEDICOS', 'U') IS NULL OR EXISTS (
          SELECT 1 FROM HORARIOS_MEDICOS HM WHERE HM.IdMedico = M.IdMedico AND HM.Estado = 1
      ));
END 
GO