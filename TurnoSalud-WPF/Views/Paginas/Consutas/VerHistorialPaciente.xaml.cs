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

namespace Views.Paginas.Consutas
{
    /// <summary>
    /// Lógica de interacción para VerHistorialPaciente.xaml
    /// </summary>
    public partial class VerHistorialPaciente : Window
    {

        private readonly HistorialController _historialController = new HistorialController();
        private readonly int _idPaciente;
        private readonly string _nombrePaciente;

        public VerHistorialPaciente(int idPaciente, string nombrePaciente)
        {
            InitializeComponent();
            _idPaciente = idPaciente;
            _nombrePaciente = nombrePaciente;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblTitulo.Text += _nombrePaciente.ToUpper();
            CargarHistorialCompleto();
        }

        private void CargarHistorialCompleto()
        {
            try
            {
                List<Historial> historialCompleto = _historialController.ListarHistorialPorPaciente(_idPaciente);

                if (historialCompleto == null || historialCompleto.Count == 0)
                {
                    new Notificacion("Este paciente no tiene un historial clínico registrado.", Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
                    this.Close();
                    return;
                }

                dgHistorial.ItemsSource = historialCompleto;

                if (dgHistorial.Items.Count > 0)
                {
                    dgHistorial.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar el historial: " + ex.Message, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
            }
        }

        private void dgHistorial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgHistorial.SelectedItem is Historial consultaSeleccionada)
            {
                txtSintomas.Text = consultaSeleccionada.Sintomas;
                txtDiagnostico.Text = consultaSeleccionada.Diagnostico;
                txtTratamiento.Text = consultaSeleccionada.Tratamiento;
                txtObservaciones.Text = consultaSeleccionada.Observaciones;

                pnlDetalle.Visibility = Visibility.Visible;
            }
            else
            {
                pnlDetalle.Visibility = Visibility.Collapsed;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) { this.Close(); }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    }
}
