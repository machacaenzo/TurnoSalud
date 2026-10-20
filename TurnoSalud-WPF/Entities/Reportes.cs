using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class DashboardTotales
    {
        public int TotalPacientes { get; set; }
        public int TotalMedicos { get; set; }
        public int TotalTurnosHoy { get; set; }
        public int TotalEspecialidades { get; set; }
    }

    public class ReporteConteo
    {
        public string Etiqueta { get; set; }
        public int Cantidad { get; set; }
    }

    public class ReporteTurno
    {
        public int IdTurno { get; set; }
        public string NombrePaciente { get; set; }
        public string NombreMedico { get; set; }
        public string Especialidad { get; set; }
        public DateTime FechaTurno { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaRegistro { get; set; }

        public string HoraInicioStr => HoraInicio.ToString("t");
        public string HoraFinStr => HoraFin.ToString("t");
    }
}
