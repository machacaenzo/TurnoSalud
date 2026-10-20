
CREATE TABLE ESPECIALIDADES (
    IdEspecialidad INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100) NOT NULL UNIQUE,
    Estado BIT NOT NULL DEFAULT 1
);
GO



CREATE PROCEDURE sp_ListarEspecialidades
    @nombre VARCHAR(100) = '', 
    @Estado BIT = 1           
AS
BEGIN
    SELECT IdEspecialidad, Nombre, Estado
    FROM ESPECIALIDADES
    WHERE Estado = @Estado 
      AND Nombre LIKE '%' + @nombre + '%';
END
GO


CREATE PROCEDURE sp_AgregarEspecialidad
    @nombre VARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM ESPECIALIDADES WHERE Nombre = @nombre)
    BEGIN
        IF EXISTS (SELECT 1 FROM ESPECIALIDADES WHERE Nombre = @nombre AND Estado = 0)
        BEGIN
            UPDATE ESPECIALIDADES SET Estado = 1 WHERE Nombre = @nombre;
        END
        ELSE
        BEGIN
            RAISERROR('Ya existe una especialidad activa con ese nombre.', 16, 1);
            RETURN;
        END
    END
    ELSE
    BEGIN
        INSERT INTO ESPECIALIDADES (Nombre, Estado) VALUES (@nombre, 1);
    END
END
GO


CREATE PROCEDURE sp_ActualizarEspecialidad
    @idEspecialidad INT,
    @nombre VARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM ESPECIALIDADES WHERE Nombre = @nombre AND IdEspecialidad <> @idEspecialidad)
    BEGIN
        RAISERROR('Ya existe otra especialidad con ese nombre.', 16, 1);
        RETURN;
    END

    UPDATE ESPECIALIDADES
    SET Nombre = @nombre
    WHERE IdEspecialidad = @idEspecialidad;
END
GO


CREATE PROCEDURE sp_DarBajaEspecialidad
    @idEspecialidad INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM MEDICOS WHERE IdEspecialidad = @idEspecialidad AND Estado = 1)
    BEGIN
        RAISERROR('No se puede dar de baja. La especialidad está asignada a uno o más médicos activos.', 16, 1);
        RETURN;
    END

    UPDATE ESPECIALIDADES
    SET Estado = 0
    WHERE IdEspecialidad = @idEspecialidad;
END
GO


CREATE PROCEDURE sp_ReactivarEspecialidad
    @idEspecialidad INT
AS
BEGIN
    DECLARE @nombreEspecialidad VARCHAR(100);
    SELECT @nombreEspecialidad = Nombre FROM ESPECIALIDADES WHERE IdEspecialidad = @idEspecialidad;
    
    IF EXISTS (SELECT 1 FROM ESPECIALIDADES WHERE Nombre = @nombreEspecialidad AND Estado = 1 AND IdEspecialidad <> @idEspecialidad)
    BEGIN
         RAISERROR('No se puede reactivar. Ya existe una especialidad activa con el mismo nombre.', 16, 1);
         RETURN;
    END

    UPDATE ESPECIALIDADES
    SET Estado = 1
    WHERE IdEspecialidad = @idEspecialidad;
END
GO