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

namespace Views.Paginas.Usuarios
{
    /// <summary>
    /// Lógica de interacción para ListaUsuarios.xaml
    /// </summary>
    public partial class ListaUsuarios : UserControl
    {
        private readonly UsuarioController usuarioController = new UsuarioController();

        public ListaUsuarios()
        {
            InitializeComponent();
            this.Loaded += OnVistaCargada;
        }

        private void OnVistaCargada(object sender, RoutedEventArgs e)
        {
            AplicarFiltrosYCargarUsuarios();
        }

        private void AplicarFiltrosYCargarUsuarios()
        {
            bool mostrarActivos = chkMostrarInactivos.IsChecked != true;
            string textoBusqueda = txtBuscar.Text;

            dgvUsuarios.ItemsSource = usuarioController.ListarUsuarios(textoBusqueda, mostrarActivos);

            columnaAccionesActivas.Visibility = mostrarActivos ? Visibility.Visible : Visibility.Collapsed;
            columnaAccionesInactivas.Visibility = mostrarActivos ? Visibility.Collapsed : Visibility.Visible;
        }

        private void chkMostrarInactivos_CheckedChanged(object sender, RoutedEventArgs e)
        {
            AplicarFiltrosYCargarUsuarios();
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltrosYCargarUsuarios();
        }

        private void btnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            GestionUsuario ventanaUsuario = new GestionUsuario();
            ventanaUsuario.ShowDialog();
            AplicarFiltrosYCargarUsuarios();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Usuario usuarioParaEditar)
            {
                GestionUsuario ventanaUsuario = new GestionUsuario();

                ventanaUsuario.InicializarParaEditar(usuarioParaEditar);

                if (ventanaUsuario.ShowDialog() == true)
                {
                    AplicarFiltrosYCargarUsuarios();
                }
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Usuario usuarioParaEliminar)
            {
                var notificacion = new Notificacion(
                    $"¿Está seguro de dar de baja a {usuarioParaEliminar.NombreCompleto}?",
                    Notificacion.TipoNotificacion.Confirmacion
                );

                if (notificacion.ShowDialog() == true)
                {
                    string rpta = usuarioController.EliminarUsuario(usuarioParaEliminar.IdUsuario);
                    if (rpta == "OK")
                    {
                        var exito = new Notificacion("Usuario Dado de Baja", Notificacion.TipoNotificacion.Exito, "El registro se ha actualizado correctamente.");
                        exito.ShowDialog();
                        AplicarFiltrosYCargarUsuarios();
                    }
                    else
                    {
                        var error = new Notificacion("Error al dar de baja", Notificacion.TipoNotificacion.Error, rpta);
                        error.ShowDialog();
                    }
                }
            }
        }

        private void btnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Usuario usuarioParaReactivar)
            {
                var notificacion = new Notificacion(
                    $"¿Está seguro de reactivar a {usuarioParaReactivar.NombreCompleto}?",
                    Notificacion.TipoNotificacion.Confirmacion
                );

                if (notificacion.ShowDialog() == true)
                {
                    string rpta = usuarioController.ReactivarUsuario(usuarioParaReactivar.IdUsuario);
                    if (rpta == "OK")
                    {
                        var exito = new Notificacion("Usuario Reactivado", Notificacion.TipoNotificacion.Exito, "El registro se ha actualizado correctamente.");
                        exito.ShowDialog();
                        AplicarFiltrosYCargarUsuarios();
                    }
                    else
                    {
                        var error = new Notificacion("Error al reactivar", Notificacion.TipoNotificacion.Error, rpta);
                        error.ShowDialog();
                    }
                }
            }
        }

        private void dgvUsuarios_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgvUsuarios.SelectedItem is Usuario usuarioSeleccionado && chkMostrarInactivos.IsChecked != true)
            {
                GestionUsuario ventanaUsuario = new GestionUsuario();

                ventanaUsuario.InicializarParaEditar(usuarioSeleccionado);

                if (ventanaUsuario.ShowDialog() == true)
                {
                    AplicarFiltrosYCargarUsuarios();
                }
            }
        }
    }
}

