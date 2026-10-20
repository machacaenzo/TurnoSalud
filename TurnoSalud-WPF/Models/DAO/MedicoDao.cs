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
    public class MedicoDao:ConexionDB
    {
        public List<Medico> ListarMedicos(string buscar, bool activos = true)
        {
            var listaMedicos = new List<Medico>();
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_ListarMedicos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@buscar", string.IsNullOrEmpty(buscar) ? "" : buscar);
                        command.Parameters.AddWithValue("@Estado", activos);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // --- CORRECCIÓN APLICADA AQUÍ ---
                                // Creamos el objeto Medico y leemos sus datos de forma segura.
                                listaMedicos.Add(new Medico
                                {
                                    // Datos directos de la tabla MEDICOS (no suelen ser nulos)
                                    IdMedico = (int)reader["IdMedico"],
                                    Matricula = reader["Matricula"].ToString(),
                                    Estado = (bool)reader["Estado"],
                                    IdEspecialidad = (int)reader["IdEspecialidad"],
                                    Especialidad = reader["Especialidad"].ToString(),

                                    // Datos de la tabla USUARIOS (a través del JOIN)
                                    IdUsuario = (int)reader["IdUsuario"],
                                    Nombre = reader["Nombre"].ToString(),
                                    Apellido = reader["Apellido"].ToString(),
                                    Dni = reader["Dni"].ToString(),
                                    NombreUsuario = reader["NombreUsuario"].ToString(),

                                    // --- LECTURA SEGURA DE CAMPOS NULOS ---
                                    Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                                    Telefono = reader["Telefono"] == DBNull.Value ? null : reader["Telefono"].ToString(),
                                    Direccion = reader["Direccion"] == DBNull.Value ? null : reader["Direccion"].ToString(),
                                    FechaNacimiento = reader["FechaNacimiento"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["FechaNacimiento"],
                                    Foto = reader["Foto"] == DBNull.Value ? null : (byte[])reader["Foto"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ListarMedicos DAO: " + ex.Message);
                throw;
            }
            return listaMedicos;
        }


        public string AgregarMedico(Medico medico)
        {
            string rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_AgregarMedico", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de Usuario (heredados)
                        command.Parameters.AddWithValue("@dni", medico.Dni);
                        command.Parameters.AddWithValue("@nombre", medico.Nombre);
                        command.Parameters.AddWithValue("@apellido", medico.Apellido);
                        command.Parameters.AddWithValue("@email", medico.Email);
                        command.Parameters.AddWithValue("@telefono", medico.Telefono);
                        command.Parameters.AddWithValue("@direccion", medico.Direccion);
                        command.Parameters.AddWithValue("@fechaNacimiento", medico.FechaNacimiento);
                        command.Parameters.AddWithValue("@nombreUsuario", medico.NombreUsuario);
                        command.Parameters.AddWithValue("@contrasena", medico.Contrasena);
                        command.Parameters.AddWithValue("@foto", medico.Foto);

                        // Parámetros de Medico
                        command.Parameters.AddWithValue("@matricula", medico.Matricula);
                        command.Parameters.AddWithValue("@idEspecialidad", medico.IdEspecialidad);

                        command.ExecuteNonQuery();
                        rpta = "OK";
                    }
                }
            }
            catch (SqlException ex) { rpta = ex.Message; }
            return rpta;
        }

        public string ActualizarMedico(Medico medico)
        {
            string rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_ActualizarMedico", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // IDs
                        command.Parameters.AddWithValue("@idUsuario", medico.IdUsuario);
                        command.Parameters.AddWithValue("@idMedico", medico.IdMedico);

                        // Parámetros de Usuario
                        command.Parameters.AddWithValue("@dni", medico.Dni);
                        command.Parameters.AddWithValue("@nombre", medico.Nombre);
                        command.Parameters.AddWithValue("@apellido", medico.Apellido);
                        command.Parameters.AddWithValue("@email", medico.Email);
                        command.Parameters.AddWithValue("@telefono", medico.Telefono);
                        command.Parameters.AddWithValue("@direccion", medico.Direccion);
                        command.Parameters.AddWithValue("@fechaNacimiento", medico.FechaNacimiento);
                        command.Parameters.AddWithValue("@nombreUsuario", medico.NombreUsuario);
                        command.Parameters.AddWithValue("@foto", medico.Foto);

                        // Parámetros de Medico
                        command.Parameters.AddWithValue("@matricula", medico.Matricula);
                        command.Parameters.AddWithValue("@idEspecialidad", medico.IdEspecialidad);

                        if (!string.IsNullOrEmpty(medico.Contrasena))
                        {
                            command.Parameters.AddWithValue("@contrasena", medico.Contrasena);
                        }

                        command.ExecuteNonQuery();
                        rpta = "OK";
                    }
                }
            }
            catch (SqlException ex) { rpta = ex.Message; }
            return rpta;
        }

        public string DarBajaMedico(int idMedico)
        {
            string rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_DarBajaMedico", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idMedico", idMedico);
                        command.ExecuteNonQuery();
                        rpta = "OK";
                    }
                }
            }
            catch (SqlException ex) { rpta = ex.Message; }
            return rpta;
        }

        public string ReactivarMedico(int idMedico)
        {
            string rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_ReactivarMedico", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idMedico", idMedico);
                        command.ExecuteNonQuery();
                        rpta = "OK";
                    }
                }
            }
            catch (SqlException ex) { rpta = ex.Message; }
            return rpta;
        }

        public List<Medico> ListarMedicosPorEspecialidad(int idEspecialidad)
        {
            var listaMedicos = new List<Medico>();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_ListarMedicosPorEspecialidad", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@idEspecialidad", idEspecialidad);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var medico = new Medico
                            {
                                IdMedico = reader.GetInt32(reader.GetOrdinal("IdMedico")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Apellido = reader.GetString(reader.GetOrdinal("Apellido"))
                            };
                            if (reader["Foto"] != DBNull.Value)
                            {
                                medico.Foto = (byte[])reader["Foto"];
                            }
                            listaMedicos.Add(medico);
                        }
                    }
                }
            }
            return listaMedicos;
        }

    }

}
