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
    public class PacienteDao :ConexionDB
    {
       
        public List<Paciente> ListarPacientes(string buscar)
        {
            var pacientes = new List<Paciente>();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_ListarPacientes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@buscar", buscar);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pacientes.Add(new Paciente
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
            return pacientes;
        }

        public Paciente BuscarPacientePorDNI(string dni)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_BuscarPacientePorDNI", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@dni", dni);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Paciente
                            {
                                IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                                DNI = reader.GetString(reader.GetOrdinal("DNI")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Apellido = reader.GetString(reader.GetOrdinal("Apellido"))
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public Paciente RegistrarPaciente(Paciente paciente)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_RegistrarPaciente", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@dni", paciente.DNI);
                    command.Parameters.AddWithValue("@nombre", paciente.Nombre);
                    command.Parameters.AddWithValue("@apellido", paciente.Apellido);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Leemos el resultado que nos devuelve el SP
                            return new Paciente
                            {
                                IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                                DNI = reader.GetString(reader.GetOrdinal("DNI")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Apellido = reader.GetString(reader.GetOrdinal("Apellido"))
                            };
                        }
                        return null;
                    }
                }
            }
        }

        
        public string ActualizarPaciente(Paciente paciente)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_ActualizarPaciente", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idPaciente", paciente.IdPaciente);
                        command.Parameters.AddWithValue("@dni", paciente.DNI); 
                        command.Parameters.AddWithValue("@nombre", paciente.Nombre);
                        command.Parameters.AddWithValue("@apellido", paciente.Apellido);

                        command.ExecuteNonQuery();
                        return "OK";
                    }
                }
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
        }


    }
}
