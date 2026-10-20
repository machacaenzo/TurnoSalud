using Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class UsuarioDao : ConexionDB
    {

        public List<Usuario> listarUsuarios(string cTexto, bool activos = true)
        {
            var listaUsuarios = new List<Usuario>();
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_ListarUsuarios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@cTexto", string.IsNullOrEmpty(cTexto) ? (object)DBNull.Value : cTexto);
                        command.Parameters.AddWithValue("@Estado", activos);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                
                                listaUsuarios.Add(new Usuario
                                {
                                    IdUsuario = (int)reader["IdUsuario"],
                                    Dni = reader["Dni"].ToString(),
                                    Nombre = reader["Nombre"].ToString(),
                                    Apellido = reader["Apellido"].ToString(),
                                    NombreUsuario = reader["NombreUsuario"].ToString(),
                                    IdRol = (int)reader["IdRol"],
                                    Rol = reader["Rol"].ToString(),
                                    Estado = (bool)reader["Estado"],

                                    
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
    
                throw;
            }
            return listaUsuarios;
        }

        public string agregarUsuario(Usuario usu)
        {
            string Rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_AgregarUsuario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@dni", usu.Dni);
                        command.Parameters.AddWithValue("@nombre", usu.Nombre);
                        command.Parameters.AddWithValue("@apellido", usu.Apellido);
                        command.Parameters.AddWithValue("@email", usu.Email);
                        command.Parameters.AddWithValue("@telefono", usu.Telefono);
                        command.Parameters.AddWithValue("@direccion", usu.Direccion);
                        command.Parameters.AddWithValue("@fechaNacimiento", usu.FechaNacimiento);
                        command.Parameters.AddWithValue("@nombreUsuario", usu.NombreUsuario);
                        command.Parameters.AddWithValue("@contrasena", usu.Contrasena);
                        command.Parameters.AddWithValue("@idRol", usu.IdRol);
                        command.Parameters.AddWithValue("@foto", usu.Foto);

                        command.ExecuteNonQuery();
                        Rpta = "OK";
                    }
                }
            }
            catch (SqlException ex)
            {                
                Rpta = "Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                Rpta = "Error inesperado: " + ex.Message;
            }
            return Rpta;
        }

        public string actualizarUsuario(Usuario usu)
        {
            string Rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_ActualizarUsuario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@idUsuario", usu.IdUsuario);
                        command.Parameters.AddWithValue("@dni", usu.Dni);
                        command.Parameters.AddWithValue("@nombre", usu.Nombre);
                        command.Parameters.AddWithValue("@apellido", usu.Apellido);
                        command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(usu.Email) ? (object)DBNull.Value : usu.Email);
                        command.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(usu.Telefono) ? (object)DBNull.Value : usu.Telefono);
                        command.Parameters.AddWithValue("@direccion", string.IsNullOrEmpty(usu.Direccion) ? (object)DBNull.Value : usu.Direccion);
                        command.Parameters.AddWithValue("@fechaNacimiento", usu.FechaNacimiento);
                        command.Parameters.AddWithValue("@nombreUsuario", usu.NombreUsuario);
                        command.Parameters.AddWithValue("@idRol", usu.IdRol);
                        command.Parameters.AddWithValue("@foto", usu.Foto ?? (object)DBNull.Value);

                        
                        if (!string.IsNullOrEmpty(usu.Contrasena))
                        {
                            command.Parameters.AddWithValue("@contrasena", usu.Contrasena);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@contrasena", (object)DBNull.Value);
                        }

                        command.ExecuteNonQuery();
                        Rpta = "OK";
                    }
                }
            }
            catch (SqlException ex)
            {
                Rpta = "Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                Rpta = "Error inesperado: " + ex.Message;
            }
            return Rpta;
        }

        public string darBajaUsuario(int idUsuario)
        {
            string Rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "sp_DarBajaUsuario";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@idUsuario", idUsuario);

                        Rpta = command.ExecuteNonQuery() >= 1 ? "OK" : "No se puede eliminar el registro";
                    }
                }

            }
            catch (Exception ex)
            {
                Rpta = "Error: " + ex.Message;
            }
            return Rpta;
        }

        public string darAltaUsuario(int idUsuario)
        {
            string Rpta = "";
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "sp_ReactivarUsuario";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@idUsuario", idUsuario);

                        Rpta = command.ExecuteNonQuery() >= 1 ? "OK" : "No se puede eliminar el registro";
                    }
                }

            }
            catch (Exception ex)
            {
                Rpta = "Error: " + ex.Message;
            }
            return Rpta;
        }

        public bool login(string nombreUsuario, string contrasena)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_Login", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    command.Parameters.AddWithValue("@contrasena", contrasena);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            UsuarioCache.IdUsuario = reader.GetInt32(0);
                            UsuarioCache.Nombre = reader.GetString(1);
                            UsuarioCache.Apellido = reader.GetString(2);
                            UsuarioCache.Dni = reader.GetString(3);
                            UsuarioCache.Email = reader.IsDBNull(4) ? null : reader.GetString(4);
                            UsuarioCache.Telefono = reader.IsDBNull(5) ? null : reader.GetString(5);
                            UsuarioCache.Direccion = reader.IsDBNull(6) ? null : reader.GetString(6);
                            UsuarioCache.FechaNacimiento = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7);
                            UsuarioCache.NombreUsuario = reader.GetString(8);
                            UsuarioCache.Foto = reader.IsDBNull(9) ? null : (byte[])reader.GetValue(9);
                            UsuarioCache.IdRol = reader.GetInt32(10);
                            UsuarioCache.Rol = reader.GetString(11);

                            UsuarioCache.IdMedico = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12);
                        }
                        return true;
                    }

                    return false;
                }
            }
        }



    }
}
