using Controllers;
using Entities;
using Microsoft.Win32;
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
using System.Windows.Shapes;
using Views.Notificaciones;

namespace Views.Paginas.Usuarios
{
    /// <summary>
    /// Lógica de interacción para GestionUsuario.xaml
    /// </summary>
    public partial class GestionUsuario : Window
    {
        private readonly UsuarioController usuarioController = new UsuarioController();
        private readonly RolController rolController = new RolController();

        private Usuario usuarioActual = new Usuario(); 
        private bool esNuevo = true;                  

        public GestionUsuario()
        {
            InitializeComponent();
            InicializarParaCrear();
        }



        public void InicializarParaCrear()
        {
            esNuevo = true;
            lblTitulo.Text = "NUEVO USUARIO";
            btnGuardar.Content = "GUARDAR";

            usuarioActual = new Usuario(); 
            LimpiarFormulario();

            imgFoto.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/user.png"));

            chkCambiarPassword.Visibility = Visibility.Collapsed;
            txtPassword.IsEnabled = true;

            CargarRolesComboBox();
        }

        public void InicializarParaEditar(Usuario usuarioAEditar)
        {
            esNuevo = false;
            lblTitulo.Text = "EDITAR USUARIO";
            btnGuardar.Content = "ACTUALIZAR";

            usuarioActual = usuarioAEditar; 

            txtDni.Text = usuarioActual.Dni;
            txtNombres.Text = usuarioActual.Nombre;
            txtApellido.Text = usuarioActual.Apellido;
            txtEmail.Text = usuarioActual.Email;
            txtTelefono.Text = usuarioActual.Telefono;
            txtDireccion.Text = usuarioActual.Direccion;
            dtpFechaNacimiento.SelectedDate = usuarioActual.FechaNacimiento;
            txtNombreUsuario.Text = usuarioActual.NombreUsuario;

            CargarImagenDesdeBytes(usuarioActual.Foto);

            chkCambiarPassword.Visibility = Visibility.Visible;
            chkCambiarPassword.IsChecked = false;
            txtPassword.IsEnabled = false;
            txtPassword.Password = ""; 

            CargarRolesComboBox();
            cmbRol.SelectedValue = usuarioActual.IdRol;
        }

       

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            usuarioActual.Dni = txtDni.Text;
            usuarioActual.Nombre = txtNombres.Text;
            usuarioActual.Apellido = txtApellido.Text;
            usuarioActual.Email = txtEmail.Text;
            usuarioActual.Telefono = txtTelefono.Text;
            usuarioActual.Direccion = txtDireccion.Text;
            usuarioActual.FechaNacimiento = dtpFechaNacimiento.SelectedDate ?? DateTime.Now;
            usuarioActual.NombreUsuario = txtNombreUsuario.Text;

            if (cmbRol.SelectedValue == null)
            {
                new Notificacion("Error de validación", Notificacion.TipoNotificacion.Error, "Debe seleccionar un rol.").ShowDialog();
                return;
            }
            usuarioActual.IdRol = (int)cmbRol.SelectedValue;

            string rpta;

            if (esNuevo)
            {
                usuarioActual.Contrasena = txtPassword.Password;
                rpta = usuarioController.AgregarUsuario(usuarioActual);
            }
            else
            {
                usuarioActual.Contrasena = (chkCambiarPassword.IsChecked == true && !string.IsNullOrEmpty(txtPassword.Password))
                                           ? txtPassword.Password
                                           : null; 
                rpta = usuarioController.ActualizarUsuario(usuarioActual);
            }

            if (rpta == "OK")
            {
                new Notificacion("Éxito", Notificacion.TipoNotificacion.Exito, "La operación se realizó correctamente.").ShowDialog();
                this.DialogResult = true; 
                this.Close();
            }
            else
            {
                new Notificacion("Error", Notificacion.TipoNotificacion.Error, rpta).ShowDialog();
            }
        }

        private void btnCambiarFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos de imagen (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    usuarioActual.Foto = File.ReadAllBytes(openFileDialog.FileName);
                    CargarImagenDesdeBytes(usuarioActual.Foto);
                }
                catch (Exception ex)
                {
                    new Notificacion("Error al cargar imagen", Notificacion.TipoNotificacion.Error, ex.Message).ShowDialog();
                }
            }
        }

        private void chkCambiarPassword_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bool habilitarCambio = chkCambiarPassword.IsChecked == true;

            txtPassword.IsEnabled = habilitarCambio;

           
            if (!habilitarCambio)
            {
                txtPassword.Password = string.Empty; 
            }
        }



        private void CargarRolesComboBox()
        {
            cmbRol.ItemsSource = rolController.ListarRolesActivos();
            cmbRol.DisplayMemberPath = "Nombre"; 
            cmbRol.SelectedValuePath = "IdRol";  
        }

        private void CargarImagenDesdeBytes(byte[] imageData)
        {
            if (imageData != null && imageData.Length > 0)
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(imageData))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                }
                imgFoto.ImageSource = bitmapImage;
            }
            else
            {
                imgFoto.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/User_default.png"));
            }
        }

        private void LimpiarFormulario()
        {
            txtDni.Text = "";
            txtNombres.Text = "";
            txtApellido.Text = "";
            txtEmail.Text = "";
            txtTelefono.Text = "";
            txtDireccion.Text = "";
            dtpFechaNacimiento.SelectedDate = null;
            txtNombreUsuario.Text = "";
            txtPassword.Password = "";
            cmbRol.SelectedIndex = -1;
        }

        

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
