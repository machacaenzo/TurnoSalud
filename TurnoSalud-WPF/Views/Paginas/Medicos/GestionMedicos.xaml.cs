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

namespace Views.Paginas.Medicos
{
    /// <summary>
    /// Lógica de interacción para GestionMedicos.xaml
    /// </summary>
    public partial class GestionMedicos : Window
    {
        private readonly MedicoController _medicoController = new MedicoController();
        private readonly EspecialidadController _especialidadController = new EspecialidadController();
        private bool _esNuevo = true;
        private Medico _medicoParaEditar;

        public GestionMedicos() { InitializeComponent(); _esNuevo = true; }
        public GestionMedicos(Medico medicoAEditar) { InitializeComponent(); _esNuevo = false; _medicoParaEditar = medicoAEditar; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarEspecialidades();
            if (!_esNuevo) CargarDatosParaEdicion();
        }

        private void CargarEspecialidades()
        {
            try
            {
                cmbEspecialidad.ItemsSource = _especialidadController.ListarEspecialidades("");
                cmbEspecialidad.DisplayMemberPath = "Nombre";
                cmbEspecialidad.SelectedValuePath = "IdEspecialidad";
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar especialidades: " + ex.Message, Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void CargarDatosParaEdicion()
        {
            lblTitulo.Text = "EDITAR MÉDICO";
            btnGuardar.Content = "Actualizar";
            txtPassword.IsEnabled = false;
            chkCambiarPassword.Visibility = Visibility.Visible;
            txtDni.Text = _medicoParaEditar.Dni;
            txtNombres.Text = _medicoParaEditar.Nombre;
            txtApellido.Text = _medicoParaEditar.Apellido;
            txtEmail.Text = _medicoParaEditar.Email;
            txtTelefono.Text = _medicoParaEditar.Telefono;
            txtDireccion.Text = _medicoParaEditar.Direccion;
            dtpFechaNacimiento.SelectedDate = _medicoParaEditar.FechaNacimiento;
            txtNombreUsuario.Text = _medicoParaEditar.NombreUsuario;
            txtMatricula.Text = _medicoParaEditar.Matricula;
            cmbEspecialidad.SelectedValue = _medicoParaEditar.IdEspecialidad;
            if (_medicoParaEditar.Foto != null && _medicoParaEditar.Foto.Length > 0)
            {
                imgFoto.ImageSource = ByteToImageSource(_medicoParaEditar.Foto);
            }
        }

        private void chkCambiarPassword_CheckedChanged(object sender, RoutedEventArgs e)
        {
            txtPassword.IsEnabled = chkCambiarPassword.IsChecked ?? false;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            string rpta;
            if (_esNuevo)
            {
                var nuevoMedico = new Medico
                {
                    Dni = txtDni.Text.Trim(),
                    Nombre = txtNombres.Text.Trim().ToUpper(),
                    Apellido = txtApellido.Text.Trim().ToUpper(),
                    Email = txtEmail.Text.Trim().ToLower(),
                    Telefono = txtTelefono.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim().ToUpper(),
                    FechaNacimiento = dtpFechaNacimiento.SelectedDate.Value,
                    NombreUsuario = txtNombreUsuario.Text.Trim(),
                    Contrasena = txtPassword.Password,
                    Matricula = txtMatricula.Text.Trim().ToUpper(),
                    IdEspecialidad = (int)cmbEspecialidad.SelectedValue,
                    Foto = ImageSourceToBytes(imgFoto.ImageSource)
                };
                rpta = _medicoController.AgregarMedico(nuevoMedico);
            }
            else
            {
                _medicoParaEditar.Dni = txtDni.Text.Trim();
                _medicoParaEditar.Nombre = txtNombres.Text.Trim().ToUpper();
                _medicoParaEditar.Apellido = txtApellido.Text.Trim().ToUpper();
                _medicoParaEditar.Email = txtEmail.Text.Trim().ToLower();
                _medicoParaEditar.Telefono = txtTelefono.Text.Trim();
                _medicoParaEditar.Direccion = txtDireccion.Text.Trim().ToUpper();
                _medicoParaEditar.FechaNacimiento = dtpFechaNacimiento.SelectedDate.Value;
                _medicoParaEditar.NombreUsuario = txtNombreUsuario.Text.Trim();
                _medicoParaEditar.Matricula = txtMatricula.Text.Trim().ToUpper();
                _medicoParaEditar.IdEspecialidad = (int)cmbEspecialidad.SelectedValue;
                _medicoParaEditar.Foto = ImageSourceToBytes(imgFoto.ImageSource);
                _medicoParaEditar.Contrasena = (chkCambiarPassword.IsChecked == true) ? txtPassword.Password : null;
                rpta = _medicoController.ActualizarMedico(_medicoParaEditar);
            }

            if (rpta == "OK")
            {
                new Notificacion(_esNuevo ? "Médico agregado correctamente." : "Médico actualizado correctamente.", Notificacion.TipoNotificacion.Exito).ShowDialog();
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                new Notificacion(rpta, Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void ResetearErrores()
        {
            errDni.Visibility = Visibility.Collapsed;
            errNombres.Visibility = Visibility.Collapsed;
            errApellido.Visibility = Visibility.Collapsed;
            errEmail.Visibility = Visibility.Collapsed;
            errTelefono.Visibility = Visibility.Collapsed;
            errDireccion.Visibility = Visibility.Collapsed;
            errFechaNacimiento.Visibility = Visibility.Collapsed;
            errNombreUsuario.Visibility = Visibility.Collapsed;
            errPassword.Visibility = Visibility.Collapsed;
            errEspecialidad.Visibility = Visibility.Collapsed;
            errMatricula.Visibility = Visibility.Collapsed;
        }

        private bool ValidarCampos()
        {
            ResetearErrores();
            bool esValido = true;

            if (string.IsNullOrWhiteSpace(txtDni.Text)) { errDni.Text = "El DNI es obligatorio."; errDni.Visibility = Visibility.Visible; esValido = false; }
            else if (!txtDni.Text.All(char.IsDigit)) { errDni.Text = "El DNI solo debe contener números."; errDni.Visibility = Visibility.Visible; esValido = false; }

            if (string.IsNullOrWhiteSpace(txtNombres.Text)) { errNombres.Text = "El nombre es obligatorio."; errNombres.Visibility = Visibility.Visible; esValido = false; }
            if (string.IsNullOrWhiteSpace(txtApellido.Text)) { errApellido.Text = "El apellido es obligatorio."; errApellido.Visibility = Visibility.Visible; esValido = false; }
            if (string.IsNullOrWhiteSpace(txtEmail.Text)) { errEmail.Text = "El email es obligatorio."; errEmail.Visibility = Visibility.Visible; esValido = false; }
            if (string.IsNullOrWhiteSpace(txtTelefono.Text)) { errTelefono.Text = "El teléfono es obligatorio."; errTelefono.Visibility = Visibility.Visible; esValido = false; }
            else if (!txtTelefono.Text.All(char.IsDigit)) { errTelefono.Text = "El teléfono solo debe contener números."; errTelefono.Visibility = Visibility.Visible; esValido = false; }
            if (string.IsNullOrWhiteSpace(txtDireccion.Text)) { errDireccion.Text = "La dirección es obligatoria."; errDireccion.Visibility = Visibility.Visible; esValido = false; }

            if (!dtpFechaNacimiento.SelectedDate.HasValue) { errFechaNacimiento.Text = "La fecha de nacimiento es obligatoria."; errFechaNacimiento.Visibility = Visibility.Visible; esValido = false; }
            else
            {
                DateTime hoy = DateTime.Today;
                if (dtpFechaNacimiento.SelectedDate.Value > hoy) { errFechaNacimiento.Text = "La fecha no puede ser futura."; errFechaNacimiento.Visibility = Visibility.Visible; esValido = false; }
                else if ((hoy.Year - dtpFechaNacimiento.SelectedDate.Value.Year) < 18) { errFechaNacimiento.Text = "Debe tener al menos 18 años."; errFechaNacimiento.Visibility = Visibility.Visible; esValido = false; }
            }

            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text)) { errNombreUsuario.Text = "El nombre de usuario es obligatorio."; errNombreUsuario.Visibility = Visibility.Visible; esValido = false; }
            if (_esNuevo || chkCambiarPassword.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Password)) { errPassword.Text = "La contraseña es obligatoria."; errPassword.Visibility = Visibility.Visible; esValido = false; }
            }

            if (cmbEspecialidad.SelectedValue == null) { errEspecialidad.Text = "Debe seleccionar una especialidad."; errEspecialidad.Visibility = Visibility.Visible; esValido = false; }
            if (string.IsNullOrWhiteSpace(txtMatricula.Text)) { errMatricula.Text = "La matrícula es obligatoria."; errMatricula.Visibility = Visibility.Visible; esValido = false; }

            return esValido;
        }

        private void btnCambiarFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Archivos de Imagen|*.jpg;*.jpeg;*.png;", Title = "Seleccione una foto" };
            if (openFileDialog.ShowDialog() == true)
            {
                try { imgFoto.ImageSource = new BitmapImage(new Uri(openFileDialog.FileName)); }
                catch (Exception ex) { new Notificacion("Error al cargar la imagen: " + ex.Message, Notificacion.TipoNotificacion.Error).ShowDialog(); }
            }
        }

        private byte[] ImageSourceToBytes(ImageSource imageSource)
        {
            if (imageSource is BitmapImage bitmapImage && bitmapImage.UriSource != null && bitmapImage.UriSource.OriginalString.Contains("user.png")) return null;
            if (imageSource == null) return null;
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageSource as BitmapSource));
            using (var stream = new MemoryStream()) { encoder.Save(stream); return stream.ToArray(); }
        }

        private ImageSource ByteToImageSource(byte[] imageData)
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

        private void btnClose_Click(object sender, RoutedEventArgs e) { this.Close(); }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    }
}
