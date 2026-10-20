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
    public class RolDao: ConexionDB
    {
        public List<Rol> ListarRolesActivos()
        {
            var listaRoles = new List<Rol>();
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_ListarRolesActivos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaRoles.Add(new Rol
                                {
                                    IdRol = (int)reader["IdRol"],
                                    Nombre = reader["Nombre"].ToString()
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
            return listaRoles;
        }
    }
}
