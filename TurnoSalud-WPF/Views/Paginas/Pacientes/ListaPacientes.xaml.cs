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
using Views.Paginas.Consutas;

namespace Views.Paginas.Pacientes
{
    /// <summary>
    /// Lógica de interacción para ListaPacientes.xaml
    /// </summary>
    public partial class ListaPacientes : UserControl
    {
        private readonly PacienteController _pacienteController = new PacienteController();

        public ListaPacientes()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarPacientes();
        }

        private void CargarPacientes()
        {
            try
            {
                dgPacientes.ItemsSource = _pacienteController.ListarPacientes(txtBuscar.Text);
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar los pacientes: " + ex.Message, Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            CargarPacientes();
        }

        private void btnNuevoPaciente_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new GestionPacientes();
            if (ventana.ShowDialog() == true)
            {
                CargarPacientes();
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Paciente paciente)
            {
                var ventana = new GestionPacientes(paciente);
                if (ventana.ShowDialog() == true)
                {
                    CargarPacientes();
                }
            }
        }


        private void btnNuevoTurno_Click(object sender, RoutedEventArgs e)
        {
            if (dgPacientes.SelectedItem is Paciente pacienteSeleccionado)
            {
                var ventanaTurno = new NuevoTurno(pacienteSeleccionado);
                ventanaTurno.ShowDialog();
            }
            else
            {
                new Notificacion("Por favor, seleccione un paciente de la lista para crear un nuevo turno.", Notificacion.TipoNotificacion.Error, "Selección Requerida").ShowDialog();
            }
        }

        private void btnVerTurnosPaciente_Click(object sender, RoutedEventArgs e)
        {
            if (dgPacientes.SelectedItem is Paciente paciente)
            {
                var ventana = new VerTurnosPaciente(paciente);
                ventana.ShowDialog();
            }
            else
            {
                new Notificacion("Por favor, seleccione un paciente de la lista para ver sus turnos.", Notificacion.TipoNotificacion.Error, "Selección Requerida").ShowDialog();
            }
        }


        private void btnHistorial_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Paciente paciente)
            {
                var ventana = new VerHistorialPaciente(paciente.IdPaciente, $"{paciente.Nombre} {paciente.Apellido}");
                ventana.Owner = Window.GetWindow(this);
                ventana.ShowDialog();
            }
        }

        private void btnVerTurnosHoy_Click(object sender, RoutedEventArgs e)
        { 
            var ventana = new VerTurnosHoy();

            ventana.Owner = Window.GetWindow(this);

            ventana.ShowDialog();
        }
    }
}
