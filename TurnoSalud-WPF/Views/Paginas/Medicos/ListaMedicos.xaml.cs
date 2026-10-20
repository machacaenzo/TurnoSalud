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

namespace Views.Paginas.Medicos
{
    /// <summary>
    /// Lógica de interacción para ListaMedicos.xaml
    /// </summary>
    public partial class ListaMedicos : UserControl
    {
        private readonly MedicoController medicoController = new MedicoController();

        public ListaMedicos()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CargarMedicos();
        }

        private void CargarMedicos()
        {
            try
            {
                bool mostrarInactivos = chkMostrarInactivos.IsChecked ?? false;
                string busqueda = txtBuscar.Text;

                List<Medico> medicos;

                if (mostrarInactivos)
                {
                    medicos = medicoController.ListarMedicosInactivos(busqueda);
                    columnaAccionesActivas.Visibility = Visibility.Collapsed;
                    columnaAccionesInactivas.Visibility = Visibility.Visible;
                    btnNuevoHorario.Visibility = Visibility.Collapsed;
                }
                else
                {
                    medicos = medicoController.ListarMedicos(busqueda);
                    columnaAccionesActivas.Visibility = Visibility.Visible;
                    columnaAccionesInactivas.Visibility = Visibility.Collapsed;
                    btnNuevoHorario.Visibility = Visibility.Visible;
                }

                dgMedicos.ItemsSource = medicos;
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar médicos: " + ex.Message, Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            CargarMedicos();
        }

        private void chkMostrarInactivos_CheckedChanged(object sender, RoutedEventArgs e)
        {
            CargarMedicos();
        }

        

        private void btnEspecialidades_Click(object sender, RoutedEventArgs e)
        {
            var ventanaEspecialidades = new GestionEspecialidades();
            ventanaEspecialidades.ShowDialog();
            CargarMedicos();
        }

        private void btnNuevoMedico_Click(object sender, RoutedEventArgs e)
        {
            var ventanaNuevoMedico = new GestionMedicos();
            if (ventanaNuevoMedico.ShowDialog() == true)
            {
                CargarMedicos();
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Medico medicoSeleccionado)
            {
                var ventanaEditarMedico = new GestionMedicos(medicoSeleccionado);
                if (ventanaEditarMedico.ShowDialog() == true)
                {
                    CargarMedicos();
                }
            }
        }


        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Medico medicoSeleccionado)
            {
                var confirmacion = new Notificacion($"¿Está seguro de que desea dar de baja al médico {medicoSeleccionado.Nombre} {medicoSeleccionado.Apellido}?", Notificacion.TipoNotificacion.Confirmacion);
                if (confirmacion.ShowDialog() == true)
                {
                    string rpta = medicoController.DarBajaMedico(medicoSeleccionado.IdMedico);
                    if (rpta == "OK")
                    {
                        new Notificacion("Médico dado de baja correctamente.", Notificacion.TipoNotificacion.Exito).ShowDialog();
                        CargarMedicos();
                    }
                    else
                    {
                        new Notificacion(rpta, Notificacion.TipoNotificacion.Error).ShowDialog();
                    }
                }
            }
        }

        private void btnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Medico medicoSeleccionado)
            {
                var confirmacion = new Notificacion($"¿Está seguro de que desea reactivar al médico {medicoSeleccionado.Nombre} {medicoSeleccionado.Apellido}?", Notificacion.TipoNotificacion.Confirmacion);
                if (confirmacion.ShowDialog() == true)
                {
                    string rpta = medicoController.ReactivarMedico(medicoSeleccionado.IdMedico);
                    if (rpta == "OK")
                    {
                        new Notificacion("Médico reactivado correctamente.", Notificacion.TipoNotificacion.Exito).ShowDialog();
                        CargarMedicos();
                    }
                    else
                    {
                        new Notificacion(rpta, Notificacion.TipoNotificacion.Error).ShowDialog();
                    }
                }
            }
        }

        private void btnNuevoHorario_Click(object sender, RoutedEventArgs e)
        {
            if (dgMedicos.SelectedItem is Medico medicoSeleccionado)
            {
                var ventanaHorario = new GestionHorario(medicoSeleccionado);
                ventanaHorario.ShowDialog();
                
            }
            else
            {
                new Notificacion("Por favor, seleccione un médico de la lista para asignarle un horario.", Notificacion.TipoNotificacion.Error, "Selección Requerida").ShowDialog();
            }
        }

        private void btnVerHorarios_Click(object sender, RoutedEventArgs e)
        {
            if (dgMedicos.SelectedItem is Medico medicoSeleccionado)
            {
                var ventanaVerHorarios = new VerHorarios(medicoSeleccionado);
                ventanaVerHorarios.ShowDialog();
            }
            else
            {
                new Notificacion("Por favor, seleccione un médico de la lista para ver sus horarios.", Notificacion.TipoNotificacion.Error, "Selección Requerida").ShowDialog();
            }
        }
    }
}
