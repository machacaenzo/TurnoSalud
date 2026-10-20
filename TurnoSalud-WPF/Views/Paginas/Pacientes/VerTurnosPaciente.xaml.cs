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
using System.Windows.Shapes;
using Views.Notificaciones;

namespace Views.Paginas.Pacientes
{
    /// <summary>
    /// Lógica de interacción para VerTurnosPaciente.xaml
    /// </summary>
    public partial class VerTurnosPaciente : Window
    {
        private readonly TurnoController _turnoController = new TurnoController();
        private readonly ReporteController _reporteController = new ReporteController();
        private readonly Paciente _paciente;

        public VerTurnosPaciente(Paciente paciente)
        {
            InitializeComponent();
            _paciente = paciente;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblTitulo.Text += _paciente.Nombre.ToUpper() + " " + _paciente.Apellido.ToUpper();
            CargarFiltros();
        }

        private void CargarFiltros()
        {
            var filtros = new List<FiltroEstadoTurno>
            {
                new FiltroEstadoTurno { NombreVisible = "Pendientes", IdEstado = 1, MostrarPasados = false },
                new FiltroEstadoTurno { NombreVisible = "Atendidos", IdEstado = 2, MostrarPasados = true },
                new FiltroEstadoTurno { NombreVisible = "Cancelados", IdEstado = 3, MostrarPasados = true },
                new FiltroEstadoTurno { NombreVisible = "Vencidos", IdEstado = 4, MostrarPasados = true },
                new FiltroEstadoTurno { NombreVisible = "Todos (Historial)", IdEstado = null, MostrarPasados = true }
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
                        _paciente.IdPaciente,
                        filtro.IdEstado,
                        filtro.MostrarPasados
                    );

                    colAccionesPendientes.Visibility = (filtro.IdEstado == 1) ? Visibility.Visible : Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    new Notificacion("Error al cargar los turnos: " + ex.Message, Notificacion.TipoNotificacion.Error).ShowDialog();
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
                var confirmacion = new Notificacion("¿Está seguro de que desea cancelar este turno?", Notificacion.TipoNotificacion.Confirmacion);
                if (confirmacion.ShowDialog() == true)
                {
                    string rpta = _turnoController.CambiarEstadoTurno(turno.IdTurno, "Cancelado");
                    if (rpta == "OK")
                    {
                        new Notificacion("Turno cancelado correctamente.", Notificacion.TipoNotificacion.Exito).ShowDialog();
                        CargarTurnos();
                    }
                    else
                    {
                        new Notificacion(rpta, Notificacion.TipoNotificacion.Error).ShowDialog();
                    }
                }
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Turno turnoAEditar)
            {
                var ventana = new NuevoTurno(_paciente, turnoAEditar);

                if (ventana.ShowDialog() == true)
                {
                    CargarTurnos();
                }
            }
        }

        private void btnComprobante_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Turno turnoSeleccionado)
            {
                try
                {
                    // 1. Obtenemos los datos completos del comprobante desde el backend
                    var turnoCompleto = _reporteController.ObtenerComprobanteTurno(turnoSeleccionado.IdTurno);
                    if (turnoCompleto == null)
                    {
                        new Notificacion("No se encontraron los datos del comprobante para este turno.", Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
                        return;
                    }

                    // 2. Creamos una instancia de nuestra nueva ventana de previsualización
                    var ventanaPreview = new VisorComprobanteTurno(turnoCompleto);

                    // 3. La mostramos como un diálogo modal
                    ventanaPreview.Owner = this;
                    ventanaPreview.ShowDialog();
                }
                catch (Exception ex)
                {
                    new Notificacion("Error al preparar el comprobante: " + ex.Message, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}
