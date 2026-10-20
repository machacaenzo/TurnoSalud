using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Turno
    {
        public int IdTurno { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public int IdFranja { get; set; }
        public DateTime FechaTurno { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdEstado { get; set; }

       
        public string NombrePaciente { get; set; }
        public string DniPaciente { get; set; }

        public string NombreMedico { get; set; }
        public string Especialidad { get; set; }

        public string Bloque { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public string EstadoNombre { get; set; }

        public string HoraInicioStr => HoraInicio.ToString(@"hh\:mm");
        public string HoraFinStr => HoraFin.ToString(@"hh\:mm");
        public string DiaNombre { get; set; } 
    }
}
