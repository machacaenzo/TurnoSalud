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
    public class HistorialDao : ConexionDB
    {
        public string RegistrarHistorial(Historial historial)
        {
            try
            {
                using (var conexion = GetConnection())
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_RegistrarHistorial", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.AddWithValue("@idTurno", historial.IdTurno);
                        comando.Parameters.AddWithValue("@sintomas", historial.Sintomas);
                        comando.Parameters.AddWithValue("@diagnostico", historial.Diagnostico);
                        comando.Parameters.AddWithValue("@tratamiento", historial.Tratamiento);
                        comando.Parameters.AddWithValue("@observaciones", historial.Observaciones);
                        comando.ExecuteNonQuery();
                        return "OK";
                    }
                }
            }
            catch (SqlException ex)
            {
                return ex.Message; // Devuelve el mensaje de error del RAISERROR
            }
        }

        public List<Historial> ListarHistorialPorPaciente(int idPaciente)
        {
            var historialCompleto = new List<Historial>();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_ListarHistorialPorPaciente", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idPaciente", idPaciente);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            historialCompleto.Add(new Historial
                            {
                                IdHistorial = reader.GetInt32(reader.GetOrdinal("IdHistorial")),
                                IdTurno = reader.GetInt32(reader.GetOrdinal("IdTurno")),
                                FechaTurno = reader.GetDateTime(reader.GetOrdinal("FechaTurno")),
                                Especialidad = reader.GetString(reader.GetOrdinal("Especialidad")),
                                NombreMedico = reader.GetString(reader.GetOrdinal("Medico")),
                                Sintomas = reader.GetString(reader.GetOrdinal("Sintomas")),
                                Diagnostico = reader.GetString(reader.GetOrdinal("Diagnostico")),
                                Tratamiento = reader.GetString(reader.GetOrdinal("Tratamiento")),
                                Observaciones = reader.GetString(reader.GetOrdinal("Observaciones")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
                            });
                        }
                    }
                }
            }
            return historialCompleto;
        }

        public Historial ObtenerHistorialPorTurno(int idTurno)
        {
            Historial historial = null;
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_ObtenerHistorialPorTurno", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idTurno", idTurno);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            historial = new Historial
                            {
                                IdHistorial = reader.GetInt32(reader.GetOrdinal("IdHistorial")),
                                IdTurno = reader.GetInt32(reader.GetOrdinal("IdTurno")),
                                Sintomas = reader.GetString(reader.GetOrdinal("Sintomas")),
                                Diagnostico = reader.GetString(reader.GetOrdinal("Diagnostico")),
                                Tratamiento = reader.GetString(reader.GetOrdinal("Tratamiento")),
                                Observaciones = reader.GetString(reader.GetOrdinal("Observaciones")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
                            };
                        }
                    }
                }
            }
            return historial;
        }

        public List<PacienteAtendido> ListarPacientesAtendidosPorMedico(int idMedico, string filtro)
        {
            var pacientesAtendidos = new List<PacienteAtendido>();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_ListarPacientesAtendidosPorMedico", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idMedico", idMedico);
                    command.Parameters.AddWithValue("@filtro", filtro);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pacientesAtendidos.Add(new PacienteAtendido
                            {
                                IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                                DNI = reader.GetString(reader.GetOrdinal("DNI")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Apellido = reader.GetString(reader.GetOrdinal("Apellido"))
                            });
                        }
                    }
                }
            }
            return pacientesAtendidos;
        }
    }
}
