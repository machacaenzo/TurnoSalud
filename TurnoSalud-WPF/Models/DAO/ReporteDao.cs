using Entities;
using Model.Dao;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DAO
{
    public class ReporteDao : ConexionDB
    {
        /// <summary>
        /// Obtiene todos los detalles de un turno para el comprobante. Devuelve la entidad 'Turno' completa.
        /// Este método es usado por el flujo de la aplicación (ej. Estación de Autogestión).
        /// </summary>
        public Turno ObtenerComprobanteTurno(int idTurno)
        {
            Turno comprobante = null;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ReporteComprobanteTurno", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idTurno", idTurno);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            comprobante = new Turno
                            {
                                IdTurno = reader.GetInt32(reader.GetOrdinal("IdTurno")),
                                IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                                DniPaciente = reader.GetString(reader.GetOrdinal("DniPaciente")),
                                NombrePaciente = reader.GetString(reader.GetOrdinal("NombrePaciente")),
                                NombreMedico = reader.GetString(reader.GetOrdinal("NombreMedico")),
                                Especialidad = reader.GetString(reader.GetOrdinal("Especialidad")),
                                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin")),
                                EstadoNombre = reader.GetString(reader.GetOrdinal("Estado")),
                                FechaTurno = reader.GetDateTime(reader.GetOrdinal("FechaTurno")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                                Bloque = reader.GetString(reader.GetOrdinal("Bloque"))
                            };
                        }
                    }
                }
            }
            return comprobante;
        }

        /// <summary>
        /// (MÉTODO AÑADIDO) Devuelve una lista de turnos dentro de un rango de fechas. Usa la entidad 'ReporteTurno'.
        /// Llama a sp_ReporteTurnosPorFecha.
        /// </summary>
        public List<ReporteTurno> ReporteTurnosPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            var reporte = new List<ReporteTurno>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ReporteTurnosPorFecha", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reporte.Add(new ReporteTurno
                            {
                                IdTurno = reader.GetInt32(reader.GetOrdinal("IdTurno")),
                                NombrePaciente = reader.GetString(reader.GetOrdinal("NombrePaciente")),
                                NombreMedico = reader.GetString(reader.GetOrdinal("NombreMedico")),
                                Especialidad = reader.GetString(reader.GetOrdinal("Especialidad")),
                                FechaTurno = reader.GetDateTime(reader.GetOrdinal("FechaTurno")),
                                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin")),
                                Estado = reader.GetString(reader.GetOrdinal("Estado")),
                                // --- LÍNEA AÑADIDA ---
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
                            });
                        }
                    }
                }
            }
            return reporte;
        }

        /// <summary>
        /// Devuelve una lista de turnos para un médico específico en un rango de fechas. Usa la entidad 'ReporteTurno'.
        /// Llama a sp_ReporteTurnosPorMedico.
        /// </summary>
        public List<ReporteTurno> ReporteTurnosPorMedico(int idMedico, DateTime fechaInicio, DateTime fechaFin)
        {
            var reporte = new List<ReporteTurno>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ReporteTurnosPorMedico", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idMedico", idMedico);
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reporte.Add(new ReporteTurno
                            {
                                IdTurno = reader.GetInt32(reader.GetOrdinal("IdTurno")),
                                NombrePaciente = reader.GetString(reader.GetOrdinal("Paciente")),
                                NombreMedico = reader.GetString(reader.GetOrdinal("NombreMedico")), // <-- LÍNEA AÑADIDA
                                Especialidad = reader.GetString(reader.GetOrdinal("Especialidad")),
                                FechaTurno = reader.GetDateTime(reader.GetOrdinal("FechaTurno")),
                                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin")),
                                Estado = reader.GetString(reader.GetOrdinal("Estado")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")) // <-- LÍNEA AÑADIDA
                            });
                        }
                    }
                }
            }
            return reporte;
        }

        /// <summary>
        /// Devuelve la cantidad de turnos agrupados por especialidad en un rango de fechas. Usa la entidad 'ReporteConteo'.
        /// Llama a sp_ReporteTurnosPorEspecialidad.
        /// </summary>
        public List<ReporteConteo> ReporteTurnosPorEspecialidad(DateTime fechaInicio, DateTime fechaFin)
        {
            var reporte = new List<ReporteConteo>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ReporteTurnosPorEspecialidad", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reporte.Add(new ReporteConteo
                            {
                                Etiqueta = reader.GetString(reader.GetOrdinal("Especialidad")),
                                Cantidad = reader.GetInt32(reader.GetOrdinal("CantidadTurnos"))
                            });
                        }
                    }
                }
            }
            return reporte;
        }

        /// <summary>
        /// Devuelve una lista de los turnos atendidos en un rango de fechas. Usa la entidad 'ReporteTurno'.
        /// Llama a sp_ReportePacientesAtendidos.
        /// </summary>
        public List<ReporteTurno> ReportePacientesAtendidos(DateTime fechaInicio, DateTime fechaFin)
        {
            var reporte = new List<ReporteTurno>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ReportePacientesAtendidos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reporte.Add(new ReporteTurno
                            {
                                NombrePaciente = reader.GetString(reader.GetOrdinal("Paciente")),
                                NombreMedico = reader.GetString(reader.GetOrdinal("Medico")),
                                Especialidad = reader.GetString(reader.GetOrdinal("Especialidad")),
                                FechaTurno = reader.GetDateTime(reader.GetOrdinal("FechaTurno")),
                                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                                Estado = reader.GetString(reader.GetOrdinal("Estado"))
                            });
                        }
                    }
                }
            }
            return reporte;
        }
    }
}
