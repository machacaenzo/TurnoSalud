using Entities;
using System;
using System.Collections.Generic;
using System.IO;
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
using Views.Paginas.Medicos;
using Views.Paginas.Pacientes;
using Views.Paginas.Principal;
using Views.Paginas.Reportes;
using Views.Paginas.Usuarios;

namespace Views
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CargarDatosUsuario();
            AplicarPermisosPorRol();

          
            if (RbPrincipal.IsVisible)
            {
                RbPrincipal.IsChecked = true;
                RbPrincipal_Click(null, null);
            }
        }

        

        private void CargarDatosUsuario()
        {
            InfoNombreUsuario.Text = UsuarioCache.NombreCompleto;
            InfoRolUsuario.Text = UsuarioCache.Rol;

            if (UsuarioCache.Foto != null && UsuarioCache.Foto.Length > 0)
            {
                InfoFotoUsuario.ImageSource = BytesToBitmapImage(UsuarioCache.Foto);
            }
        }

        private void AplicarPermisosPorRol()
        {
            RbPrincipal.Visibility = Visibility.Collapsed;
            RbUsuarios.Visibility = Visibility.Collapsed;
            RbMedicos.Visibility = Visibility.Collapsed;
            RbPacientes.Visibility = Visibility.Collapsed;
            RbConsulta.Visibility = Visibility.Collapsed;
            RbReportes.Visibility = Visibility.Collapsed;

            string rol = UsuarioCache.Rol?.ToUpper() ?? "";

            switch (rol)
            {
                case "ADMINISTRADOR":
                    RbPrincipal.Visibility = Visibility.Visible;
                    RbUsuarios.Visibility = Visibility.Visible;
                    RbMedicos.Visibility = Visibility.Visible;
                    RbPacientes.Visibility = Visibility.Visible;
                    RbConsulta.Visibility = Visibility.Visible;
                    RbReportes.Visibility = Visibility.Visible;
                    break;
               
                case "MEDICO":
                    RbPrincipal.Visibility = Visibility.Visible;
                    RbConsulta.Visibility = Visibility.Visible;
                    break;
                case "RECEPCIONISTA":
                    RbPrincipal.Visibility = Visibility.Visible;
                    RbMedicos.Visibility = Visibility.Visible;
                    RbPacientes.Visibility = Visibility.Visible;
                    RbReportes.Visibility = Visibility.Visible;
                    break;
            }
        }

        

        private void RbCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            UsuarioCache.Logout();
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void RbPrincipal_Click(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = "Dashboard Principal";
            MainContent.Content = new Dashboard();
        }

        private void RbUsuarios_Click(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = "Gestión de Usuarios";
            MainContent.Content = new ListaUsuarios();
        }

        private void RbMedicos_Click(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = "Gestión de Médicos";
            MainContent.Content = new ListaMedicos();
        }

        private void RbPacientes_Click(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = "Gestión de Pacientes";
            MainContent.Content = new ListaPacientes();
        }

        private void RbConsulta_Click(object sender, RoutedEventArgs e)
        {
          
            if (UsuarioCache.Rol?.ToUpper() == "MEDICO" && UsuarioCache.IdMedico.HasValue)
            {
                PageTitle.Text = "Agenda de Consultas";
                MainContent.Content = new Consultas();
            }
            else
            {
                new Notificacion("Acceso Denegado", Notificacion.TipoNotificacion.Error, "Solo los usuarios con el rol de Médico pueden acceder a esta sección.").ShowDialog();

                var botonActivo = menuContainer.Children.OfType<RadioButton>().FirstOrDefault(rb => rb.IsChecked == true && rb != RbConsulta);
                if (botonActivo != null)
                {
                    botonActivo.IsChecked = true;
                }
                else 
                {
                    RbPrincipal.IsChecked = true;
                }

                RbConsulta.IsChecked = false;
            }
        }

        private void RbReportes_Click(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = "Reportes";
            MainContent.Content = new Reportes();
        }

      
        private BitmapImage BytesToBitmapImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;

            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}
