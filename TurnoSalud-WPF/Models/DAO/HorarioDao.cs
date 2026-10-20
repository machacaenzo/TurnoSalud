using Model.Dao;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Models.DAO
{
    public class HorarioDao:ConexionDB
    {
        public List<HorarioDetalle> ListarHorariosDeMedico(int idMedico)
        {
            var horariosDetallados = new List<HorarioDetalle>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ListarHorariosDeMedico", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idMedico", idMedico);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            horariosDetallados.Add(new HorarioDetalle
                            {
                                DiaSemana = reader.GetInt32(reader.GetOrdinal("DiaSemana")),
                                Bloque = reader.GetString(reader.GetOrdinal("Bloque")),
                                IdFranja = reader.GetInt32(reader.GetOrdinal("IdFranja")),
                                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin"))
                            });
                        }
                    }
                }
            }
            return horariosDetallados;
        }

        public string DarBajaHorarioMedico(int idMedico, int idFranja, int diaSemana)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_DarBajaHorarioMedico", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idMedico", idMedico);
                        command.Parameters.AddWithValue("@idFranja", idFranja);
                        command.Parameters.AddWithValue("@diaSemana", diaSemana);
                        command.ExecuteNonQuery();
                    }
                }
                return "OK";
            }
            catch (SqlException ex) { return ex.Message; }
        }


        public string AsignarHorariosMedico(int idMedico, List<FranjaAsignacion> asignaciones)
        {
            try
            {
                var dtAsignaciones = new DataTable();
                dtAsignaciones.Columns.Add("IdFranja", typeof(int));
                dtAsignaciones.Columns.Add("DiaSemana", typeof(int));
                foreach (var franja in asignaciones)
                {
                    dtAsignaciones.Rows.Add(franja.IdFranja, franja.DiaSemana);
                }

                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_AsignarHorariosMedico", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idMedico", idMedico);
                        var param = new SqlParameter("@asignaciones", SqlDbType.Structured) { TypeName = "FranjaAsignacion", Value = dtAsignaciones };
                        command.Parameters.Add(param);
                        command.ExecuteNonQuery();
                    }
                }
                return "OK";
            }
            catch (SqlException ex) { return ex.Message; }
        }

        public List<BloqueHorario> ListarBloquesHorario()
        {
            var bloques = new List<BloqueHorario>();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_ListarBloquesHorario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bloques.Add(new BloqueHorario
                            {
                                IdBloque = reader.GetInt32(reader.GetOrdinal("IdBloque")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre"))
                            });
                        }
                    }
                }
            }
            return bloques;
        }

        public List<FranjaHoraria> ListarFranjasPorBloque(int idBloque)
        {
            var franjas = new List<FranjaHoraria>();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_ListarFranjasPorBloque", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idBloque", idBloque);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            franjas.Add(new FranjaHoraria
                            {
                                IdFranja = reader.GetInt32(reader.GetOrdinal("IdFranja")),
                                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin"))
                            });
                        }
                    }
                }
            }
            return franjas;
        }


        public List<HorarioDetalle> ListarHorariosDisponiblesDelMedico(int idMedico, DateTime fecha)
        {
            var horariosDisponibles = new List<HorarioDetalle>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ListarHorariosDisponiblesDelMedico", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idMedico", idMedico);
                    cmd.Parameters.AddWithValue("@fechaReferencia", fecha);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            horariosDisponibles.Add(new HorarioDetalle
                            {
                                DiaSemana = reader.GetInt32(reader.GetOrdinal("DiaSemana")),
                                Bloque = reader.GetString(reader.GetOrdinal("Bloque")),
                                IdFranja = reader.GetInt32(reader.GetOrdinal("IdFranja")),
                                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin"))
                            });
                        }
                    }
                }
            }
            return horariosDisponibles;
        }
    }
}
