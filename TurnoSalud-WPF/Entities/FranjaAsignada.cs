using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class BloqueHorario
    {
        public int IdBloque { get; set; }
        public string Nombre { get; set; }
    }
    public class FranjaHoraria
    {
        public int IdFranja { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string HoraInicioStr => HoraInicio.ToString(@"hh\:mm");
        public string HoraFinStr => HoraFin.ToString(@"hh\:mm");
    }
    public class FranjaAsignacion
    {
        public int IdFranja { get; set; }
        public int DiaSemana { get; set; }
    }

}
