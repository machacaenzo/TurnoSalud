using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class FiltroEstadoTurno
    {
        public string NombreVisible { get; set; } 
        public int? IdEstado { get; set; } 
        public bool MostrarPasados { get; set; }
    }
}
