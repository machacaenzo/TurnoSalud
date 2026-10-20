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
    public class DashboardDao : ConexionDB
    {
        public DashboardTotales ObtenerTotalesDashboard()
        {
            var totales = new DashboardTotales();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_ObtenerTotalesDashboard", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@totalPacientes", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@totalMedicos", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@totalTurnosHoy", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@totalEspecialidades", SqlDbType.Int).Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery(); 

                    // Leer los valores de los parámetros de salida
                    totales.TotalPacientes = (int)cmd.Parameters["@totalPacientes"].Value;
                    totales.TotalMedicos = (int)cmd.Parameters["@totalMedicos"].Value;
                    totales.TotalTurnosHoy = (int)cmd.Parameters["@totalTurnosHoy"].Value;
                    totales.TotalEspecialidades = (int)cmd.Parameters["@totalEspecialidades"].Value;
                }
            }
            return totales;
        }

        public List<ReporteConteo> Top5EspecialidadesConMasTurnos()
        {
            var lista = new List<ReporteConteo>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_Top5EspecialidadesConMasTurnos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new ReporteConteo
                            {
                                Etiqueta = reader.GetString(reader.GetOrdinal("Especialidad")),
                                Cantidad = reader.GetInt32(reader.GetOrdinal("CantidadTurnos"))
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public List<ReporteConteo> Top5MedicosConMasTurnos()
        {
            var lista = new List<ReporteConteo>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_Top5MedicosConMasTurnos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new ReporteConteo
                            {
                                Etiqueta = reader.GetString(reader.GetOrdinal("Medico")),
                                Cantidad = reader.GetInt32(reader.GetOrdinal("CantidadTurnos"))
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public List<Turno> Ultimos5TurnosAtendidos()
        {
            var lista = new List<Turno>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_Ultimos5TurnosAtendidos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Turno
                            {
                                FechaTurno = reader.GetDateTime(reader.GetOrdinal("FechaTurno")),
                                NombrePaciente = reader.GetString(reader.GetOrdinal("Paciente")),
                                NombreMedico = reader.GetString(reader.GetOrdinal("Medico")),
                                Especialidad = reader.GetString(reader.GetOrdinal("Especialidad")),
                                EstadoNombre = reader.GetString(reader.GetOrdinal("Estado"))
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}
