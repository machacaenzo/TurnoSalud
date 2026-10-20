using Entities;
using Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class DashboardController
    {
        private readonly DashboardDao _dashboardDao = new DashboardDao();

        public DashboardTotales ObtenerTotalesDashboard()
        {
            return _dashboardDao.ObtenerTotalesDashboard();
        }

        public List<ReporteConteo> Top5EspecialidadesConMasTurnos()
        {
            return _dashboardDao.Top5EspecialidadesConMasTurnos();
        }

        public List<ReporteConteo> Top5MedicosConMasTurnos()
        {
            return _dashboardDao.Top5MedicosConMasTurnos();
        }

        public List<Turno> Ultimos5TurnosAtendidos()
        {
            return _dashboardDao.Ultimos5TurnosAtendidos();
        }
    }
}
