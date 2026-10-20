using Entities;
using Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class HistorialController
    {

        private readonly HistorialDao _historialDao = new HistorialDao();

        public string RegistrarHistorial(Historial historial)
        {
            return _historialDao.RegistrarHistorial(historial);
        }

        public List<Historial> ListarHistorialPorPaciente(int idPaciente)
        {
            return _historialDao.ListarHistorialPorPaciente(idPaciente);
        }

        public Historial ObtenerHistorialPorTurno(int idTurno)
        {
            return _historialDao.ObtenerHistorialPorTurno(idTurno);
        }

        public List<PacienteAtendido> ListarPacientesAtendidosPorMedico(int idMedico, string filtro)
        {
            return _historialDao.ListarPacientesAtendidosPorMedico(idMedico, filtro);
        }
    }
}
