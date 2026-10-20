CREATE TABLE ROLES (
    IdRol INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(50) NOT NULL UNIQUE,
    Estado BIT NOT NULL DEFAULT 1
);
GO

IF NOT EXISTS (SELECT 1 FROM ROLES WHERE Nombre = 'Administrador')
    INSERT INTO ROLES (Nombre) VALUES ('Administrador');
IF NOT EXISTS (SELECT 1 FROM ROLES WHERE Nombre = 'Médico')
    INSERT INTO ROLES (Nombre) VALUES ('Médico');
IF NOT EXISTS (SELECT 1 FROM ROLES WHERE Nombre = 'Recepcionista')
    INSERT INTO ROLES (Nombre) VALUES ('Recepcionista');
GO


CREATE PROCEDURE sp_ListarRolesActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdRol, Nombre
    FROM ROLES
    WHERE Estado = 1
    ORDER BY Nombre;
END
GO

CREATE TABLE USUARIOS (
    IdUsuario INT PRIMARY KEY IDENTITY(1,1),
    Dni VARCHAR(15) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Email VARCHAR(100),
    Telefono VARCHAR(30),
    Direccion VARCHAR(200),
    FechaNacimiento DATE,
    NombreUsuario VARCHAR(50) NOT NULL,
    Contrasena VARCHAR(100) NOT NULL,
    IdRol INT NOT NULL,
    Estado BIT NOT NULL DEFAULT 1,
    Foto VARBINARY(MAX) NULL,   
    
    FOREIGN KEY (IdRol) REFERENCES ROLES(IdRol)
);
GO



CREATE PROCEDURE sp_AgregarUsuario
    @dni VARCHAR(15),
    @nombre VARCHAR(100),
    @apellido VARCHAR(100),
    @email VARCHAR(100),
    @telefono VARCHAR(30),
    @direccion VARCHAR(200),
    @fechaNacimiento DATE,
    @nombreUsuario VARCHAR(50),
    @contrasena VARCHAR(100),
    @idRol INT,
    @foto VARBINARY(MAX)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM USUARIOS WHERE Dni = @dni)
    BEGIN
        RAISERROR('Ya existe un usuario con ese DNI.', 16, 1);
        RETURN;
    END
    IF EXISTS (SELECT 1 FROM USUARIOS WHERE NombreUsuario = @nombreUsuario)
    BEGIN
        RAISERROR('Ya existe un usuario con ese Nombre de Usuario.', 16, 1);
        RETURN;
    END

    INSERT INTO USUARIOS (Dni, Nombre, Apellido, Email, Telefono, Direccion, FechaNacimiento, NombreUsuario, Contrasena, IdRol, Estado, Foto)
    VALUES (@dni, @nombre, @apellido, @email, @telefono, @direccion, @fechaNacimiento, @nombreUsuario, @contrasena, @idRol, 1, @foto);
END;
GO


CREATE PROCEDURE sp_ActualizarUsuario
    @idUsuario INT,
    @dni VARCHAR(15),
    @nombre VARCHAR(100),
    @apellido VARCHAR(100),
    @email VARCHAR(100),
    @telefono VARCHAR(30),
    @direccion VARCHAR(200),
    @fechaNacimiento DATE,
    @nombreUsuario VARCHAR(50),
    @idRol INT,
    @foto VARBINARY(MAX),
    @contrasena VARCHAR(100) = NULL 
AS
BEGIN
    IF EXISTS (SELECT 1 FROM USUARIOS WHERE Dni = @dni AND IdUsuario <> @idUsuario)
    BEGIN
        RAISERROR('Ya existe otro usuario con ese DNI.', 16, 1);
        RETURN;
    END
    IF EXISTS (SELECT 1 FROM USUARIOS WHERE NombreUsuario = @nombreUsuario AND IdUsuario <> @idUsuario)
    BEGIN
        RAISERROR('Ya existe otro usuario con ese Nombre de Usuario.', 16, 1);
        RETURN;
    END

    UPDATE USUARIOS
    SET 
        Dni = @dni,
        Nombre = @nombre,
        Apellido = @apellido,
        Email = @email,
        Telefono = @telefono,
        Direccion = @direccion,
        FechaNacimiento = @fechaNacimiento,
        NombreUsuario = @nombreUsuario,
        IdRol = @idRol,
        Foto = @foto
    WHERE IdUsuario = @idUsuario;

    IF @contrasena IS NOT NULL AND LTRIM(RTRIM(@contrasena)) <> ''
    BEGIN
        UPDATE USUARIOS
        SET Contrasena = @contrasena
        WHERE IdUsuario = @idUsuario;
    END
END
GO


CREATE PROCEDURE sp_ListarUsuarios
    @cTexto NVARCHAR(255) = NULL,
    @Estado BIT = 1
AS
BEGIN
    SELECT 
        u.IdUsuario, u.Dni, u.Nombre, u.Apellido, u.NombreUsuario,
        u.Email, u.Telefono, u.Direccion, u.FechaNacimiento, u.IdRol,
        r.Nombre AS Rol, u.Estado, u.Foto
    FROM USUARIOS u
    INNER JOIN ROLES r ON u.IdRol = r.IdRol
    WHERE u.Estado = @Estado
      AND (
            @cTexto IS NULL OR LTRIM(RTRIM(@cTexto)) = '' OR
            u.Nombre LIKE '%' + @cTexto + '%' OR
            u.Apellido LIKE '%' + @cTexto + '%' OR
            u.NombreUsuario LIKE '%' + @cTexto + '%' OR
            u.Email LIKE '%' + @cTexto + '%' OR
            u.Telefono LIKE '%' + @cTexto + '%' OR
            u.Dni LIKE '%' + @cTexto + '%' OR
            r.Nombre LIKE '%' + @cTexto + '%'
          );
END
GO


ALTER PROCEDURE sp_DarBajaUsuario
    @idUsuario INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM MEDICOS WHERE IdUsuario = @idUsuario AND Estado = 1)
    BEGIN
        DECLARE @idMedico INT;
        DECLARE @idEstadoPendiente INT;

        SELECT @idMedico = IdMedico FROM MEDICOS WHERE IdUsuario = @idUsuario;
        
        SELECT @idEstadoPendiente = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Pendiente';

        IF EXISTS (
            SELECT 1 FROM TURNOS
            WHERE IdMedico = @idMedico
              AND IdEstado = @idEstadoPendiente 
              AND FechaTurno >= CAST(GETDATE() AS DATE)
        )
        BEGIN
            RAISERROR('No se puede dar de baja. El médico tiene turnos futuros pendientes.', 16, 1);
            RETURN;
        END

        UPDATE MEDICOS SET Estado = 0 WHERE IdUsuario = @idUsuario;
    END

    UPDATE USUARIOS SET Estado = 0 WHERE IdUsuario = @idUsuario;
END
GO

CREATE PROCEDURE sp_ReactivarUsuario
    @idUsuario INT
AS
BEGIN
    UPDATE USUARIOS SET Estado = 1 WHERE IdUsuario = @idUsuario;

    IF EXISTS (SELECT 1 FROM MEDICOS WHERE IdUsuario = @idUsuario)
    BEGIN
        UPDATE MEDICOS SET Estado = 1 WHERE IdUsuario = @idUsuario;
    END
END
GO


CREATE PROCEDURE sp_Login
    @nombreUsuario VARCHAR(50),
    @contrasena VARCHAR(100)
AS
BEGIN
    SELECT 
        U.IdUsuario, U.Nombre, U.Apellido, U.Dni, U.Email,
        U.Telefono, U.Direccion, U.FechaNacimiento, U.NombreUsuario,
        U.Foto, U.IdRol, R.Nombre AS Rol, M.IdMedico
    FROM USUARIOS U
    INNER JOIN ROLES R ON U.IdRol = R.IdRol
    LEFT JOIN MEDICOS M ON U.IdUsuario = M.IdUsuario
    WHERE 
        U.NombreUsuario = @nombreUsuario 
        AND U.Contrasena = @contrasena
        AND U.Estado = 1;
END
GO