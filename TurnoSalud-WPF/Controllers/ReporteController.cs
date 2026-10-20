using Entities;
using Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class ReporteController
    {
        private readonly ReporteDao _reporteDao = new ReporteDao();

        
        public Turno ObtenerComprobanteTurno(int idTurno)
        {
            return _reporteDao.ObtenerComprobanteTurno(idTurno);
        }

       
        public List<ReporteTurno> ReporteTurnosPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            return _reporteDao.ReporteTurnosPorFecha(fechaInicio, fechaFin);
        }

       
        public List<ReporteTurno> ReporteTurnosPorMedico(int idMedico, DateTime fechaInicio, DateTime fechaFin)
        {
            return _reporteDao.ReporteTurnosPorMedico(idMedico, fechaInicio, fechaFin);
        }

       
        public List<ReporteConteo> ReporteTurnosPorEspecialidad(DateTime fechaInicio, DateTime fechaFin)
        {
            return _reporteDao.ReporteTurnosPorEspecialidad(fechaInicio, fechaFin);
        }

        
        public List<ReporteTurno> ReportePacientesAtendidos(DateTime fechaInicio, DateTime fechaFin)
        {
            return _reporteDao.ReportePacientesAtendidos(fechaInicio, fechaFin);
        }
    }
}
