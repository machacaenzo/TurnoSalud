using Entities;
using Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class PacienteController
    {
        private readonly PacienteDao _pacienteDao = new PacienteDao();

        public List<Paciente> ListarPacientes(string buscar)
        {
            return _pacienteDao.ListarPacientes(buscar);
        }
        public Paciente BuscarPacientePorDNI(string dni)
        {
            return _pacienteDao.BuscarPacientePorDNI(dni);
        }
        public Paciente RegistrarPaciente(Paciente paciente)
        {
            return _pacienteDao.RegistrarPaciente(paciente);
        }

        public string ActualizarPaciente(Paciente paciente)
        {
            return _pacienteDao.ActualizarPaciente(paciente);
        }
    }
}
