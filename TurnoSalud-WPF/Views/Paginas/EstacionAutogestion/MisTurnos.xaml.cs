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

namespace Views.Paginas.EstacionAutogestion
{
    /// <summary>
    /// Lógica de interacción para MisTurnos.xaml
    /// </summary>
    public partial class MisTurnos : Page
    {
        private readonly TurnoController _turnoController = new TurnoController();
        private readonly int _idPaciente;
        private readonly string _nombrePaciente;

        public MisTurnos(int idPaciente, string nombrePaciente)
        {
            InitializeComponent();
            _idPaciente = idPaciente;
            _nombrePaciente = nombrePaciente;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lblNombrePaciente.Text = _nombrePaciente;
            CargarFiltros();
        }

        private void CargarFiltros()
        {
            var filtros = new List<FiltroEstadoTurno>
            {
                new FiltroEstadoTurno { NombreVisible = "Mis Próximos Turnos (Pendientes)", IdEstado = 1, MostrarPasados = false },
                new FiltroEstadoTurno { NombreVisible = "Turnos Atendidos", IdEstado = 2, MostrarPasados = true },
                new FiltroEstadoTurno { NombreVisible = "Turnos Cancelados", IdEstado = 3, MostrarPasados = true },
                new FiltroEstadoTurno { NombreVisible = "Ver Todo el Historial", IdEstado = null, MostrarPasados = true }
            };

            cmbEstado.ItemsSource = filtros;
            cmbEstado.DisplayMemberPath = "NombreVisible";
            cmbEstado.SelectedIndex = 0;
        }

        private void CargarTurnos()
        {
            if (cmbEstado.SelectedItem is FiltroEstadoTurno filtro)
            {
                try
                {
                    dgTurnos.ItemsSource = _turnoController.ListarTurnosPorPaciente(
                        _idPaciente,
                        filtro.IdEstado,
                        filtro.MostrarPasados
                    );

                    colAccionCancelar.Visibility = (filtro.IdEstado == 1) ? Visibility.Visible : Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    new Notificaciones.Notificacion("Error al cargar los turnos: " + ex.Message, Notificaciones.Notificacion.TipoNotificacion.Error).ShowDialog();
                }
            }
        }

        private void cmbEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTurnos();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Turno turno)
            {
                var confirmacion = new Notificaciones.Notificacion("¿Está seguro de que desea cancelar este turno?", Notificaciones.Notificacion.TipoNotificacion.Confirmacion);
                if (confirmacion.ShowDialog() == true)
                {
                    string rpta = _turnoController.CambiarEstadoTurno(turno.IdTurno, "Cancelado");
                    if (rpta == "OK")
                    {
                        new Notificaciones.Notificacion("Turno cancelado correctamente.", Notificaciones.Notificacion.TipoNotificacion.Exito).ShowDialog();
                        CargarTurnos();
                    }
                    else
                    {
                        new Notificaciones.Notificacion(rpta, Notificaciones.Notificacion.TipoNotificacion.Error).ShowDialog();
                    }
                }
            }
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
