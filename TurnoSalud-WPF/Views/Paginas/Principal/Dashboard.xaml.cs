using Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Views.Notificaciones;

namespace Views.Paginas.Principal
{
    /// <summary>
    /// Lógica de interacción para Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        private readonly DashboardController _dashboardController = new DashboardController();
        private readonly TurnoController _turnoController = new TurnoController();

        public Dashboard()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ActualizarTurnosVencidos();
            CargarDatosDashboard();
            
        }

        private void CargarDatosDashboard()
        {
            try
            {
                
              

               
                var totales = _dashboardController.ObtenerTotalesDashboard();
                lblTotalPacientes.Text = totales.TotalPacientes.ToString();
                lblTotalMedicos.Text = totales.TotalMedicos.ToString();
                lblTotalTurnosHoy.Text = totales.TotalTurnosHoy.ToString();
                lblTotalEspecialidades.Text = totales.TotalEspecialidades.ToString();

                dgTopEspecialidades.ItemsSource = _dashboardController.Top5EspecialidadesConMasTurnos();
                dgTopMedicos.ItemsSource = _dashboardController.Top5MedicosConMasTurnos();
                dgUltimosTurnos.ItemsSource = _dashboardController.Ultimos5TurnosAtendidos();
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar los datos del Dashboard: " + ex.Message, Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        
        private void ActualizarTurnosVencidos()
        {
            try
            {
                string resultado = _turnoController.MarcarTurnosVencidos();

                
                if (resultado != "OK")
                {
                    Debug.WriteLine("Error al actualizar turnos vencidos: " + resultado);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Excepción al llamar a ActualizarTurnosVencidos: " + ex.Message);
            }
        }
    }
}
