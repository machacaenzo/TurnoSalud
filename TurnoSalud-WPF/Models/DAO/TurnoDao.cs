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
    public class TurnosDao : ConexionDB
    {
        public int RegistrarTurno(Turno turno)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_RegistrarTurno", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idPaciente", turno.IdPaciente);
                        command.Parameters.AddWithValue("@idMedico", turno.IdMedico);
                        command.Parameters.AddWithValue("@idFranja", turno.IdFranja);
                        command.Parameters.AddWithValue("@fecha", turno.FechaTurno);

                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public string CambiarEstadoTurno(int idTurno, string nuevoEstadoNombre)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_CambiarEstadoTurno", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idTurno", idTurno);
                        command.Parameters.AddWithValue("@nuevoEstadoNombre", nuevoEstadoNombre);
                        command.ExecuteNonQuery();
                        return "OK";
                    }
                }
            }
            catch (SqlException ex) { return ex.Message; }
        }

        public string ModificarTurno(Turno turno)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_ModificarTurno", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idTurno", turno.IdTurno);
                        command.Parameters.AddWithValue("@idMedico", turno.IdMedico);
                        command.Parameters.AddWithValue("@idFranja", turno.IdFranja);
                        command.Parameters.AddWithValue("@fecha", turno.FechaTurno);
                        command.ExecuteNonQuery();
                        return "OK";
                    }
                }
            }
            catch (SqlException ex) { return ex.Message; }
        }

        public List<Turno> ListarTurnosPorMedico(int idMedico, DateTime fecha, int idEstado)
        {
            var turnos = new List<Turno>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ListarTurnosPorMedico", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idMedico", idMedico);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@idEstado", idEstado);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            turnos.Add(new Turno
                            {
                                IdTurno = reader.GetInt32(reader.GetOrdinal("IdTurno")),
                                NombrePaciente = reader.GetString(reader.GetOrdinal("Paciente")),
                                Bloque = reader.GetString(reader.GetOrdinal("Bloque")),
                                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin")),
                                FechaTurno = reader.GetDateTime(reader.GetOrdinal("FechaTurno")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                                EstadoNombre = reader.GetString(reader.GetOrdinal("Estado"))
                            });
                        }
                    }
                }
            }
            return turnos;
        }

        public List<Turno> ListarTurnosPorPaciente(int idPaciente, int? idEstado, bool mostrarPasados)
        {
            var turnos = new List<Turno>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ListarTurnosPorPaciente", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idPaciente", idPaciente);
                    cmd.Parameters.AddWithValue("@idEstado", (object)idEstado ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@mostrarPasados", mostrarPasados);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            turnos.Add(new Turno
                            {
                                IdTurno = reader.GetInt32(reader.GetOrdinal("IdTurno")),
                                IdFranja = reader.GetInt32(reader.GetOrdinal("IdFranja")),
                                DiaNombre = reader.GetString(reader.GetOrdinal("DiaNombre")),
                                NombreMedico = reader.GetString(reader.GetOrdinal("Medico")),
                                Especialidad = reader.GetString(reader.GetOrdinal("Especialidad")),
                                Bloque = reader.GetString(reader.GetOrdinal("Bloque")),
                                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin")),
                                FechaTurno = reader.GetDateTime(reader.GetOrdinal("FechaTurno")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                                EstadoNombre = reader.GetString(reader.GetOrdinal("Estado"))
                            });
                        }
                    }
                }
            }
            return turnos;
        }

        public List<Turno> ListarTurnosDelDia()
        {
            var turnos = new List<Turno>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ListarTurnosDelDia", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            turnos.Add(new Turno
                            {
                                IdTurno = reader.GetInt32(reader.GetOrdinal("IdTurno")),
                                DniPaciente = reader.GetString(reader.GetOrdinal("DNI")),
                                NombrePaciente = reader.GetString(reader.GetOrdinal("Paciente")),
                                Bloque = reader.GetString(reader.GetOrdinal("Bloque")),
                                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin")),
                                FechaTurno = reader.GetDateTime(reader.GetOrdinal("FechaTurno")),
                                EstadoNombre = reader.GetString(reader.GetOrdinal("Estado"))
                            });
                        }
                    }
                }
            }
            return turnos;
        }
        public string MarcarTurnosVencidos()
        {
            string Rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_MarcarTurnosVencidos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();
                        Rpta = "OK";
                    }
                }
            }
            catch (Exception ex)
            {
                Rpta = "Error: " + ex.Message;
            }
            return Rpta;
        }
    }
}
