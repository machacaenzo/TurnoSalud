using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class HorarioDetalle
    {
        public int DiaSemana { get; set; }
        public string DiaNombre => Dias[DiaSemana];
        public string Bloque { get; set; }
        public int IdFranja { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public DateTime FechaTurno { get; set; }
        public string HoraInicioStr => HoraInicio.ToString(@"hh\:mm");
        public string HoraFinStr => HoraFin.ToString(@"hh\:mm");
        private static readonly string[] Dias = { "", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
    }
 
}
