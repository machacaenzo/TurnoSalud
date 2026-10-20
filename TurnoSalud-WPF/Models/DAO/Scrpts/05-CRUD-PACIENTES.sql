

CREATE TABLE PACIENTES (
    IdPaciente INT PRIMARY KEY IDENTITY(1,1),
    DNI VARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL
);
GO


CREATE PROCEDURE sp_ListarPacientes
    @buscar NVARCHAR(100) = ''
AS
BEGIN
    SELECT IdPaciente, DNI, Nombre, Apellido
    FROM PACIENTES
    WHERE Nombre LIKE '%' + @buscar + '%'
       OR Apellido LIKE '%' + @buscar + '%'
       OR DNI LIKE '%' + @buscar + '%';
END
GO

CREATE PROCEDURE sp_BuscarPacientePorDNI
    @dni VARCHAR(20)
AS
BEGIN
    SELECT IdPaciente, DNI, Nombre, Apellido
    FROM PACIENTES
    WHERE DNI = @dni;
END
GO

CREATE PROCEDURE sp_RegistrarPaciente
    @dni VARCHAR(20),
    @nombre NVARCHAR(100),
    @apellido NVARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM PACIENTES WHERE DNI = @dni)
    BEGIN
        SELECT IdPaciente, DNI, Nombre, Apellido FROM PACIENTES WHERE DNI = @dni;
        RETURN;
    END
    ELSE
    BEGIN
        INSERT INTO PACIENTES (DNI, Nombre, Apellido)
        VALUES (@dni, @nombre, @apellido);
        
        SELECT IdPaciente, DNI, Nombre, Apellido FROM PACIENTES WHERE IdPaciente = SCOPE_IDENTITY();
    END
END
GO

CREATE PROCEDURE sp_ActualizarPaciente
    @idPaciente INT,
    @dni VARCHAR(20), 
    @nombre NVARCHAR(100),
    @apellido NVARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM PACIENTES WHERE DNI = @dni AND IdPaciente <> @idPaciente)
    BEGIN
        RAISERROR('El DNI ingresado ya está registrado para otro paciente.', 16, 1);
        RETURN;
    END

    UPDATE PACIENTES
    SET DNI = @dni,
        Nombre = @nombre,
        Apellido = @apellido
    WHERE IdPaciente = @idPaciente;
END
GO