
using Entities;
using Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class RolController
    {
        private readonly RolDao rolDao = new RolDao();

        public List<Rol> ListarRolesActivos()
        {
            return rolDao.ListarRolesActivos();
        }
    }
}
