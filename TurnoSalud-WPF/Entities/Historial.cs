using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Historial
    {
        public int IdHistorial { get; set; }
        public int IdTurno { get; set; }
        public string Sintomas { get; set; }
        public string Diagnostico { get; set; }
        public string Tratamiento { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }

       
        public DateTime FechaTurno { get; set; }
        public string Especialidad { get; set; }
        public string NombreMedico { get; set; }
    }
    public class PacienteAtendido
    {
        public int IdPaciente { get; set; }
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
    }
}
