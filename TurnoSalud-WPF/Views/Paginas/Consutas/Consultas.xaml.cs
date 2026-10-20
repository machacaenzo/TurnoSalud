using Controllers;
using Entities;
using System;
using System.Collections.Generic;
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

namespace Views.Paginas.Consutas
{
    /// <summary>
    /// Lógica de interacción para Consultas.xaml
    /// </summary>
    public partial class Consultas : UserControl
    {
        private readonly TurnoController _turnoController = new TurnoController();
        private readonly int _idMedicoLogueado = UsuarioCache.IdMedico.Value;

        // --- AÑADIMOS CONSTANTES PARA MEJORAR LA LEGIBILIDAD Y EVITAR ERRORES ---
        private const int ID_ESTADO_PENDIENTE = 1;
        private const int ID_ESTADO_ATENDIDO = 2; // <-- ¡ESTE ERA EL VALOR CORRECTO!

        public Consultas()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dpFecha.SelectedDate = DateTime.Today;
            CargarTurnos();
        }

        private void CargarTurnos()
        {
            try
            {
                bool mostrarAtendidos = chkMostrarAtendidos.IsChecked ?? false;

                if (mostrarAtendidos)
                {
                    DateTime fechaSeleccionada = dpFecha.SelectedDate ?? DateTime.Today;
                    // --- CORRECCIÓN APLICADA AQUÍ ---
                    // Usamos la constante para el ID del estado 'Atendido'
                    dgTurnos.ItemsSource = _turnoController.ListarTurnosPorMedico(_idMedicoLogueado, fechaSeleccionada, ID_ESTADO_ATENDIDO);
                    colAccionConsulta.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Usamos la constante para el ID del estado 'Pendiente'
                    dgTurnos.ItemsSource = _turnoController.ListarTurnosPorMedico(_idMedicoLogueado, DateTime.Today, ID_ESTADO_PENDIENTE);
                    colAccionConsulta.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar los turnos: " + ex.Message, Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void chkMostrarAtendidos_CheckedChanged(object sender, RoutedEventArgs e)
        {
            dpFecha.Visibility = (chkMostrarAtendidos.IsChecked ?? false) ? Visibility.Visible : Visibility.Collapsed;
            CargarTurnos();
        }

        private void dpFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (chkMostrarAtendidos.IsChecked ?? false)
            {
                CargarTurnos();
            }
        }

        private void btnRegistrarConsulta_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Turno turno)
            {
                var ventana = new RegistrarConsulta(turno) { Owner = Window.GetWindow(this) };
                if (ventana.ShowDialog() == true)
                {
                    CargarTurnos();
                }
            }
        }

        private void btnVerMisPacientes_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new MisPacientes(_idMedicoLogueado) { Owner = Window.GetWindow(this) };
            ventana.ShowDialog();
        }
    }
}
