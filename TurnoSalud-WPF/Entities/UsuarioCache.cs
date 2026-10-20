using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class UsuarioCache
    {
        public static int IdUsuario { get; set; }
        public static string Dni { get; set; }
        public static string Nombre { get; set; }
        public static string Apellido { get; set; }
        public static string Email { get; set; }
        public static string Telefono { get; set; }
        public static string Direccion { get; set; }
        public static DateTime FechaNacimiento { get; set; }
        public static string NombreUsuario { get; set; }
        public static int IdRol { get; set; }
        public static bool Estado { get; set; }
        public static byte[] Foto { get; set; }
        public static string Rol { get; set; }
        public static int? IdMedico { get; set; }
        public static string NombreCompleto => $"{Nombre} {Apellido}";
        public static void Logout()
        {
            IdUsuario = 0;
            Dni = null;
            Nombre = null;
            Apellido = null;
            Email = null;
            Telefono = null;
            Direccion = null;
            FechaNacimiento = DateTime.MinValue;
            NombreUsuario = null;
            IdRol = 0;
            Estado = false;
            Foto = null;
            Rol = null;
            IdMedico = null;
        }
    }

}
