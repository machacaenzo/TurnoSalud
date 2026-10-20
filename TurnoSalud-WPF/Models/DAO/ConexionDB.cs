using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class ConexionDB
    {
        private readonly string connectionString;

        public ConexionDB()
        {
            connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\ENZOMACHACA\\Documents\\APU2025\\LPOOII\\TRABAJOSPRACTICOS\\Enzo-Nahuel-Machaca-TPN3-LPOOII\\BDD\\TurnoSalud-Wpf.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True";
        }
        protected SqlConnection GetConnection()
        {
           return new SqlConnection(connectionString);
           
        }
    }
}
