using Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Dao;

namespace Models.DAO
{
    public class EspecialidadDao:ConexionDB
    {
        public List<Especialidad> ListarEspecialidades(string nombre, bool estado = true)
        {
            List<Especialidad> listaEspecialidades = new List<Especialidad>();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_ListarEspecialidades", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@nombre", nombre);
                    command.Parameters.AddWithValue("@Estado", estado);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaEspecialidades.Add(new Especialidad
                            {
                                IdEspecialidad = reader.GetInt32(reader.GetOrdinal("IdEspecialidad")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("Estado"))
                            });
                        }
                    }
                }
            }
            return listaEspecialidades;
        }

       
        public string AgregarEspecialidad(Especialidad especialidad)
        {
            string rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_AgregarEspecialidad", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@nombre", especialidad.Nombre);

                        int resultado = command.ExecuteNonQuery();
                        rpta = resultado >= 1 ? "OK" : "No se pudo completar la operación.";
                    }
                }
            }
            catch (SqlException ex)
            {
                rpta = ex.Message;
            }
            return rpta;
        }

       
        public string ActualizarEspecialidad(Especialidad especialidad)
        {
            string rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_ActualizarEspecialidad", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idEspecialidad", especialidad.IdEspecialidad);
                        command.Parameters.AddWithValue("@nombre", especialidad.Nombre);

                        rpta = command.ExecuteNonQuery() >= 1 ? "OK" : "No se pudo actualizar el registro.";
                    }
                }
            }
            catch (SqlException ex)
            {
                rpta = ex.Message;
            }
            return rpta;
        }

       
        public string DarBajaEspecialidad(int idEspecialidad)
        {
            string rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_DarBajaEspecialidad", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idEspecialidad", idEspecialidad);
                        command.ExecuteNonQuery();
                        rpta = "OK";
                    }
                }
            }
            catch (SqlException ex)
            {
                rpta = ex.Message;
            }
            return rpta;
        }

      
        public string ReactivarEspecialidad(int idEspecialidad)
        {
            string rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_ReactivarEspecialidad", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idEspecialidad", idEspecialidad);

                        rpta = command.ExecuteNonQuery() >= 1 ? "OK" : "No se pudo reactivar el registro.";
                    }
                }
            }
            catch (SqlException ex)
            {
                rpta = ex.Message;
            }
            return rpta;
        }
    }
}
