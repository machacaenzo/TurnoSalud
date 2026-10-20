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
    /// Lógica de interacción para MisPacientes.xaml
    /// </summary>
    public partial class MisPacientes : Window
    {
        private readonly HistorialController _historialController = new HistorialController();
        private readonly int _idMedico;

        public MisPacientes(int idMedico)
        {
            InitializeComponent();
            _idMedico = idMedico;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarPacientes();
        }

        private void CargarPacientes()
        {
            try
            {
                // Llamamos al método del controlador que busca en el historial
                dgPacientes.ItemsSource = _historialController.ListarPacientesAtendidosPorMedico(_idMedico, txtBuscar.Text);
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar la lista de pacientes: " + ex.Message, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
            }
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            CargarPacientes();
        }


        private void btnVerHistorial_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is PacienteAtendido paciente)
            {
                // Creamos la nueva ventana pasándole el ID y el nombre completo del paciente
                var ventana = new VerHistorialPaciente(paciente.IdPaciente, $"{paciente.Nombre} {paciente.Apellido}");
                ventana.Owner = this;
                ventana.ShowDialog();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) { this.Close(); }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    }
}

