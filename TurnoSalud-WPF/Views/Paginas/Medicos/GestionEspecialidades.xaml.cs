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

namespace Views.Paginas.Medicos
{
    /// <summary>
    /// Lógica de interacción para GestionEspecialidades.xaml
    /// </summary>
    public partial class GestionEspecialidades : Window
    {
        private readonly EspecialidadController _controller = new EspecialidadController();
        private bool _esNuevo = true;

        public GestionEspecialidades()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarEspecialidades();
            ModoNuevo(); 
        }

        #region Lógica de Carga y UI

        private void CargarEspecialidades()
        {
            try
            {
                bool mostrarInactivos = chkMostrarInactivos.IsChecked ?? false;
                dgEspecialidades.ItemsSource = mostrarInactivos
                    ? _controller.ListarEspecialidadesInactivas(txtBuscar.Text)
                    : _controller.ListarEspecialidades(txtBuscar.Text);

                
                bool habilitarFormulario = !mostrarInactivos;
                txtNombre.IsEnabled = habilitarFormulario;
                btnGuardar.IsEnabled = habilitarFormulario;
                btnEditar.IsEnabled = habilitarFormulario; 

                ModoNuevo(); 
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar las especialidades: " + ex.Message, Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void ModoNuevo()
        {
            _esNuevo = true;
            txtNombre.Clear();
            dgEspecialidades.SelectedItem = null;
            btnGuardar.Content = "Guardar";
            txtNombre.Focus();
            ActualizarEstadoBotones();
        }

        private void ModoEditar()
        {
            if (dgEspecialidades.SelectedItem is Especialidad especialidad)
            {
                _esNuevo = false;
                txtNombre.Text = especialidad.Nombre;
                btnGuardar.Content = "Actualizar";
                ActualizarEstadoBotones();
            }
        }

        private void ActualizarEstadoBotones()
        {
            Especialidad seleccionada = dgEspecialidades.SelectedItem as Especialidad;
            bool haySeleccion = seleccionada != null;

            btnEditar.IsEnabled = haySeleccion && !(chkMostrarInactivos.IsChecked ?? false);
            btnEliminar.IsEnabled = haySeleccion;

            if (haySeleccion)
            {
                btnEliminar.Content = seleccionada.Estado ? "Eliminar" : "Reactivar";
            }
            else
            {
                btnEliminar.Content = "Eliminar";
            }
        }

        private void dgEspecialidades_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActualizarEstadoBotones();
            
            if (!_esNuevo)
            {
                ModoNuevo();
            }
        }

        #endregion

        #region Eventos de Botones

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            ModoNuevo();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgEspecialidades.SelectedItem != null)
            {
                ModoEditar();
                txtNombre.Focus();
                txtNombre.SelectAll();
            }
            else
            {
                new Notificacion("Por favor, seleccione una especialidad de la lista para editar.", Notificacion.TipoNotificacion.Error, "Selección Requerida").ShowDialog();
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                new Notificacion("El nombre de la especialidad no puede estar vacío.", Notificacion.TipoNotificacion.Error).ShowDialog();
                return;
            }

            string rpta;
            string nombreEspecialidad = txtNombre.Text.ToUpper().Trim();

            if (_esNuevo)
            {
                var nuevaEspecialidad = new Especialidad { Nombre = nombreEspecialidad };
                rpta = _controller.AgregarEspecialidad(nuevaEspecialidad);
            }
            else
            {
                if (dgEspecialidades.SelectedItem is Especialidad especialidadParaEditar)
                {
                    especialidadParaEditar.Nombre = nombreEspecialidad;
                    rpta = _controller.ActualizarEspecialidad(especialidadParaEditar);
                }
                else
                {
                    new Notificacion("No se ha seleccionado ninguna especialidad para actualizar.", Notificacion.TipoNotificacion.Error).ShowDialog();
                    return;
                }
            }

            if (rpta == "OK")
            {
                new Notificacion(_esNuevo ? "Especialidad agregada correctamente." : "Especialidad actualizada correctamente.", Notificacion.TipoNotificacion.Exito).ShowDialog();
                CargarEspecialidades(); 
            }
            else
            {
                new Notificacion(rpta, Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgEspecialidades.SelectedItem is Especialidad especialidadSeleccionada)
            {
                bool esActivo = especialidadSeleccionada.Estado;
                string accion = esActivo ? "dar de baja" : "reactivar";
                string rpta;

                var confirmacion = new Notificacion($"¿Está seguro de que desea {accion} la especialidad '{especialidadSeleccionada.Nombre}'?", Notificacion.TipoNotificacion.Confirmacion);

                if (confirmacion.ShowDialog() == true)
                {
                    rpta = esActivo
                        ? _controller.DarBajaEspecialidad(especialidadSeleccionada.IdEspecialidad)
                        : _controller.ReactivarEspecialidad(especialidadSeleccionada.IdEspecialidad);

                    if (rpta == "OK")
                    {
                        new Notificacion($"Especialidad {accion}da correctamente.", Notificacion.TipoNotificacion.Exito).ShowDialog();
                        CargarEspecialidades(); 
                    }
                    else
                    {
                        new Notificacion(rpta, Notificacion.TipoNotificacion.Error).ShowDialog();
                    }
                }
            }
        }

        #endregion

        #region Eventos de Búsqueda y Ventana

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            CargarEspecialidades();
        }

        private void chkMostrarInactivos_CheckedChanged(object sender, RoutedEventArgs e)
        {
            CargarEspecialidades();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        #endregion
    }
}
