using Entities;
using Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class MedicoController
    {
        private MedicoDao medicoDao = new MedicoDao();

        public List<Medico> ListarMedicos(string buscar)
        {
            return medicoDao.ListarMedicos(buscar, true);
        }

        public List<Medico> ListarMedicosInactivos(string buscar)
        {
            return medicoDao.ListarMedicos(buscar, false);
        }

        public string AgregarMedico(Medico medico)
        {
            return medicoDao.AgregarMedico(medico);
        }

        public string ActualizarMedico(Medico medico)
        {
            return medicoDao.ActualizarMedico(medico);
        }

        public string DarBajaMedico(int idMedico)
        {
            return medicoDao.DarBajaMedico(idMedico);
        }

        public string ReactivarMedico(int idMedico)
        {
            return medicoDao.ReactivarMedico(idMedico);
        }

        public List<Medico> ListarMedicosPorEspecialidad(int idEspecialidad)
        {
            return medicoDao.ListarMedicosPorEspecialidad(idEspecialidad);
        }
    }
}
