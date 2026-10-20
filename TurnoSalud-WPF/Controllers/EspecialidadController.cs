using Entities;
using Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class EspecialidadController
    {
        EspecialidadDao especialidadDao = new EspecialidadDao();

        public List<Especialidad> ListarEspecialidades(string nombre)
        {
            return especialidadDao.ListarEspecialidades(nombre, true);
        }

        public List<Especialidad> ListarEspecialidadesInactivas(string nombre)
        {
            return especialidadDao.ListarEspecialidades(nombre, false);
        }

        public string AgregarEspecialidad(Especialidad especialidad)
        {
            return especialidadDao.AgregarEspecialidad(especialidad);
        }

        public string ActualizarEspecialidad(Especialidad especialidad)
        {
            return especialidadDao.ActualizarEspecialidad(especialidad);
        }

        public string DarBajaEspecialidad(int idEspecialidad)
        {
            return especialidadDao.DarBajaEspecialidad(idEspecialidad);
        }

        public string ReactivarEspecialidad(int idEspecialidad)
        {
            return especialidadDao.ReactivarEspecialidad(idEspecialidad);
        }
    }
}
