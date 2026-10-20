ALTER PROCEDURE sp_ReporteComprobanteTurno
    @idTurno INT
AS
BEGIN
    SELECT 
        T.IdTurno,
        P.IdPaciente, 
        P.DNI AS DniPaciente,
        P.Nombre + ' ' + P.Apellido AS NombrePaciente,
        U.Nombre + ' ' + U.Apellido AS NombreMedico,
        E.Nombre AS Especialidad,
        FH.HoraInicio,
        FH.HoraFin,
        BH.Nombre AS Bloque,
        T.FechaTurno,
        ET.Nombre AS Estado,
        T.FechaRegistro
    FROM TURNOS T
    INNER JOIN PACIENTES P ON T.IdPaciente = P.IdPaciente
    INNER JOIN MEDICOS M ON T.IdMedico = M.IdMedico
    INNER JOIN USUARIOS U ON M.IdUsuario = U.IdUsuario
    INNER JOIN ESPECIALIDADES E ON M.IdEspecialidad = E.IdEspecialidad
    INNER JOIN FRANJA_HORARIO FH ON T.IdFranja = FH.IdFranja
    INNER JOIN BLOQUE_HORARIO BH ON FH.IdBloque = BH.IdBloque
    INNER JOIN ESTADOS_TURNO ET ON T.IdEstado = ET.IdEstado
    WHERE T.IdTurno = @idTurno;
END
GO

ALTER PROCEDURE sp_ReporteTurnosPorFecha
    @fechaInicio DATE,
    @fechaFin DATE
AS
BEGIN
    SELECT 
        T.IdTurno,
        P.Nombre + ' ' + P.Apellido AS NombrePaciente,
        U.Nombre + ' ' + U.Apellido AS NombreMedico,
        E.Nombre AS Especialidad,
        T.FechaTurno,
        FH.HoraInicio,
        FH.HoraFin,
        ET.Nombre AS Estado,
        T.FechaRegistro 
    FROM TURNOS T
    INNER JOIN PACIENTES P ON T.IdPaciente = P.IdPaciente
    INNER JOIN MEDICOS M ON T.IdMedico = M.IdMedico
    INNER JOIN USUARIOS U ON M.IdUsuario = U.IdUsuario
    INNER JOIN ESPECIALIDADES E ON M.IdEspecialidad = E.IdEspecialidad
    INNER JOIN FRANJA_HORARIO FH ON T.IdFranja = FH.IdFranja
    INNER JOIN ESTADOS_TURNO ET ON T.IdEstado = ET.IdEstado
    WHERE T.FechaTurno BETWEEN @fechaInicio AND @fechaFin
    ORDER BY T.FechaTurno, FH.HoraInicio;
END
GO

ALTER PROCEDURE sp_ReporteTurnosPorMedico
    @idMedico INT,
    @fechaInicio DATE,
    @fechaFin DATE
AS
BEGIN
    SELECT 
        T.IdTurno,
        P.Nombre + ' ' + P.Apellido AS Paciente,
        U.Nombre + ' ' + U.Apellido AS NombreMedico, 
        E.Nombre AS Especialidad,
        T.FechaTurno,
        FH.HoraInicio,
        FH.HoraFin,
        ET.Nombre AS Estado,
        T.FechaRegistro 
    FROM TURNOS T
    INNER JOIN PACIENTES P ON T.IdPaciente = P.IdPaciente
    INNER JOIN MEDICOS M ON T.IdMedico = M.IdMedico
    INNER JOIN USUARIOS U ON M.IdUsuario = U.IdUsuario
    INNER JOIN ESPECIALIDADES E ON M.IdEspecialidad = E.IdEspecialidad
    INNER JOIN FRANJA_HORARIO FH ON T.IdFranja = FH.IdFranja
    INNER JOIN ESTADOS_TURNO ET ON T.IdEstado = ET.IdEstado
    WHERE T.IdMedico = @idMedico 
      AND T.FechaTurno BETWEEN @fechaInicio AND @fechaFin
    ORDER BY T.FechaTurno, FH.HoraInicio;
END
GO

CREATE PROCEDURE sp_ReporteTurnosPorEspecialidad
    @fechaInicio DATE, @fechaFin DATE
AS
BEGIN
    SELECT E.Nombre AS Especialidad, COUNT(*) AS CantidadTurnos
    FROM TURNOS T
    INNER JOIN MEDICOS M ON T.IdMedico = M.IdMedico
    INNER JOIN ESPECIALIDADES E ON M.IdEspecialidad = E.IdEspecialidad
    WHERE T.FechaTurno BETWEEN @fechaInicio AND @fechaFin
    GROUP BY E.Nombre
    ORDER BY CantidadTurnos DESC;
END
GO

ALTER PROCEDURE sp_ReportePacientesAtendidos
    @fechaInicio DATE,
    @fechaFin DATE
AS
BEGIN
    DECLARE @idEstadoAtendido INT;
    SELECT @idEstadoAtendido = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Atendido';

    SELECT 
        P.Nombre + ' ' + P.Apellido AS Paciente,
        U.Nombre + ' ' + U.Apellido AS Medico,
        E.Nombre AS Especialidad,
        T.FechaTurno,
        FH.HoraInicio,
        FH.HoraFin,
        T.FechaRegistro,
        ET.Nombre AS Estado 
    FROM TURNOS T
    INNER JOIN PACIENTES P ON T.IdPaciente = P.IdPaciente
    INNER JOIN MEDICOS M ON T.IdMedico = M.IdMedico
    INNER JOIN USUARIOS U ON M.IdUsuario = U.IdUsuario
    INNER JOIN ESPECIALIDADES E ON M.IdEspecialidad = E.IdEspecialidad
    INNER JOIN FRANJA_HORARIO FH ON T.IdFranja = FH.IdFranja
    INNER JOIN ESTADOS_TURNO ET ON T.IdEstado = ET.IdEstado 
    WHERE 
        T.IdEstado = @idEstadoAtendido 
        AND T.FechaTurno BETWEEN @fechaInicio AND @fechaFin
    ORDER BY T.FechaTurno, FH.HoraInicio;
END
GO

CREATE PROCEDURE sp_ObtenerTotalesDashboard
    @totalPacientes INT OUTPUT, @totalMedicos INT OUTPUT,
    @totalTurnosHoy INT OUTPUT, @totalEspecialidades INT OUTPUT
AS
BEGIN
    DECLARE @idEstadoAtendido INT;
    SELECT @idEstadoAtendido = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Atendido';

    SET @totalPacientes = (SELECT COUNT(*) FROM PACIENTES);
    SET @totalMedicos = (SELECT COUNT(*) FROM MEDICOS WHERE Estado = 1);
    SET @totalTurnosHoy = (SELECT COUNT(*) FROM TURNOS WHERE FechaTurno = CAST(GETDATE() AS DATE) AND IdEstado = @idEstadoAtendido);
    SET @totalEspecialidades = (SELECT COUNT(*) FROM ESPECIALIDADES WHERE Estado = 1);
END
GO

CREATE PROCEDURE sp_Top5EspecialidadesConMasTurnos
AS
BEGIN
    DECLARE @idEstadoAtendido INT;
    SELECT @idEstadoAtendido = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Atendido';

    SELECT TOP 5 E.Nombre AS Especialidad, COUNT(*) AS CantidadTurnos
    FROM TURNOS T
    INNER JOIN MEDICOS M ON T.IdMedico = M.IdMedico
    INNER JOIN ESPECIALIDADES E ON M.IdEspecialidad = E.IdEspecialidad
    WHERE T.IdEstado = @idEstadoAtendido
    GROUP BY E.Nombre
    ORDER BY CantidadTurnos DESC;
END
GO

CREATE PROCEDURE sp_Top5MedicosConMasTurnos
AS
BEGIN
    DECLARE @idEstadoAtendido INT;
    SELECT @idEstadoAtendido = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Atendido';

    SELECT TOP 5 U.Nombre + ' ' + U.Apellido AS Medico, COUNT(*) AS CantidadTurnos
    FROM TURNOS T
    INNER JOIN MEDICOS M ON T.IdMedico = M.IdMedico
    INNER JOIN USUARIOS U ON M.IdUsuario = U.IdUsuario
    WHERE T.IdEstado = @idEstadoAtendido
    GROUP BY U.Nombre, U.Apellido
    ORDER BY CantidadTurnos DESC;
END
GO

CREATE PROCEDURE sp_Ultimos5TurnosAtendidos
AS
BEGIN
    DECLARE @idEstadoAtendido INT;
    SELECT @idEstadoAtendido = IdEstado FROM ESTADOS_TURNO WHERE Nombre = 'Atendido';
    
    SELECT TOP 5 T.FechaTurno, P.Nombre + ' ' + P.Apellido AS Paciente,
           U.Nombre + ' ' + U.Apellido AS Medico, E.Nombre AS Especialidad,
           ET.Nombre AS Estado
    FROM TURNOS T
    INNER JOIN PACIENTES P ON T.IdPaciente = P.IdPaciente
    INNER JOIN MEDICOS M ON T.IdMedico = M.IdMedico
    INNER JOIN USUARIOS U ON M.IdUsuario = U.IdUsuario
    INNER JOIN ESPECIALIDADES E ON M.IdEspecialidad = E.IdEspecialidad
    INNER JOIN ESTADOS_TURNO ET ON T.IdEstado = ET.IdEstado
    WHERE T.IdEstado = @idEstadoAtendido
    ORDER BY T.FechaTurno DESC, T.FechaRegistro DESC;
END
GO

CREATE PROCEDURE sp_TopEspecialidadesPorMedicos
AS
BEGIN
    SELECT TOP 3 E.Nombre AS Especialidad, COUNT(M.IdMedico) AS Cantidad
    FROM MEDICOS M
    INNER JOIN ESPECIALIDADES E ON M.IdEspecialidad = E.IdEspecialidad
    WHERE M.Estado = 1
    GROUP BY E.Nombre
    ORDER BY Cantidad DESC;
END
GO