using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Medico : Usuario
    {
        public int IdMedico { get; set; }
        public string Matricula { get; set; }
        public int IdEspecialidad { get; set; }

       
        public string Especialidad { get; set; }
    }
}
