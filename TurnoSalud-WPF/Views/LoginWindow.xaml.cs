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

namespace Views
{
    /// <summary>
    /// Lógica de interacción para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly UsuarioController usuarioController = new UsuarioController();
        private bool esContrasenaVisible = false;

        public LoginWindow()
        {
            InitializeComponent();
            txtUsuario.Focus();
        }

        private void btnIngresar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblMensajeError.Text = "";
                string usuario = txtUsuario.Text.Trim();
                string contrasena = esContrasenaVisible ? txtPasswordVisible.Text : txtPassword.Password;

                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
                {
                    lblMensajeError.Text = "DEBE INGRESAR EL USUARIO Y CONTRASEÑA";
                    return;
                }

                bool loginCorrecto = usuarioController.Login(usuario, contrasena);

                if (loginCorrecto)
                {
                    var notificacionExito = new Notificacion("Inicio de Sesión Exitoso", Notificacion.TipoNotificacion.Exito, $"Bienvenido {UsuarioCache.NombreCompleto}");
                    notificacionExito.ShowDialog();

                   
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();

                    this.Close();
                    
                }
                else
                {
                    lblMensajeError.Text = "USUARIO O CONTRASEÑA INCORRECTOS";
                    txtPassword.Clear();
                    txtPasswordVisible.Clear();
                    txtUsuario.Focus();
                }
            }
            catch (System.Exception ex)
            {
                var notificacionError = new Notificacion("Error Inesperado", Notificacion.TipoNotificacion.Error, ex.Message);
                notificacionError.ShowDialog();
            }
        }


        private void btnVerContrasena_Click(object sender, RoutedEventArgs e)
        {
            esContrasenaVisible = !esContrasenaVisible;
            if (esContrasenaVisible)
            {
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
                txtPasswordVisible.Text = txtPassword.Password;
                txtPasswordVisible.Focus();
                IconoVerContrasena.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.EyeOff;
            }
            else
            {
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Visible;
                txtPassword.Password = txtPasswordVisible.Text;
                txtPassword.Focus();
                IconoVerContrasena.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Eye;
            }
        }

      

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
