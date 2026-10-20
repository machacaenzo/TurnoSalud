using Entities;
using Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class HorarioController
    {
        private readonly HorarioDao _horarioDao = new HorarioDao();

      
        public List<HorarioDetalle> ListarHorariosDeMedico(int idMedico)
        {
            return _horarioDao.ListarHorariosDeMedico(idMedico);
        }

       
        public string DarBajaHorarioMedico(int idMedico, int idFranja, int diaSemana)
        {
            return _horarioDao.DarBajaHorarioMedico(idMedico, idFranja, diaSemana);
        }

     
        public string AsignarHorariosMedico(int idMedico, List<FranjaAsignacion> asignaciones)
        {
            return _horarioDao.AsignarHorariosMedico(idMedico, asignaciones);
        }

       
        public List<BloqueHorario> ListarBloquesHorario()
        {
            return _horarioDao.ListarBloquesHorario();
        }

        
        public List<FranjaHoraria> ListarFranjasPorBloque(int idBloque)
        {
            return _horarioDao.ListarFranjasPorBloque(idBloque);
        }

        
        public List<HorarioDetalle> ListarHorariosDisponiblesDelMedico(int idMedico, DateTime fecha)
        {
            return _horarioDao.ListarHorariosDisponiblesDelMedico(idMedico, fecha);
        }
    }
}
