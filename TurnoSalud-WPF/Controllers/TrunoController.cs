using Entities;
using Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class TurnoController
    {
        private readonly TurnosDao _turnosDao = new TurnosDao();

        
        public int RegistrarTurno(Turno turno)
        {
            return _turnosDao.RegistrarTurno(turno);
        }

        
        public string CambiarEstadoTurno(int idTurno, string nuevoEstadoNombre)
        {
            return _turnosDao.CambiarEstadoTurno(idTurno, nuevoEstadoNombre);
        }

        
        public string ModificarTurno(Turno turno)
        {
            return _turnosDao.ModificarTurno(turno);
        }

        
        public List<Turno> ListarTurnosPorMedico(int idMedico, DateTime fecha, int idEstado)
        {
            return _turnosDao.ListarTurnosPorMedico(idMedico, fecha, idEstado);
        }

       
        public List<Turno> ListarTurnosPorPaciente(int idPaciente, int? idEstado, bool mostrarPasados)
        {
            return _turnosDao.ListarTurnosPorPaciente(idPaciente, idEstado, mostrarPasados);
        }

        public List<Turno> ListarTurnosDelDia()
        {
            return _turnosDao.ListarTurnosDelDia();
        }
        public string MarcarTurnosVencidos()
        {
            return _turnosDao.MarcarTurnosVencidos();
        }
    }
}
