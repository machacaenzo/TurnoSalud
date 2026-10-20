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

namespace Views.Paginas.Pacientes
{
    /// <summary>
    /// Lógica de interacción para GestionPacientes.xaml
    /// </summary>
    public partial class GestionPacientes : Window
    {
        private readonly PacienteController _pacienteController = new PacienteController();
        private bool _esNuevo = true;
        private Paciente _pacienteParaEditar;

        public GestionPacientes()
        {
            InitializeComponent();
            _esNuevo = true;
        }

        public GestionPacientes(Paciente pacienteAEditar)
        {
            InitializeComponent();
            _esNuevo = false;
            _pacienteParaEditar = pacienteAEditar;
            CargarDatosParaEdicion();
        }

        private void CargarDatosParaEdicion()
        {
            lblTitulo.Text = "EDITAR PACIENTE";
            btnGuardar.Content = "Actualizar";
            txtDni.Text = _pacienteParaEditar.DNI;
            txtNombre.Text = _pacienteParaEditar.Nombre;
            txtApellido.Text = _pacienteParaEditar.Apellido;
            txtDni.IsEnabled = false; 
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            if (_esNuevo)
            {
                var nuevoPaciente = new Paciente
                {
                    DNI = txtDni.Text.Trim(),
                    Nombre = txtNombre.Text.Trim().ToUpper(),
                    Apellido = txtApellido.Text.Trim().ToUpper()
                };

                Paciente pacienteRegistrado = _pacienteController.RegistrarPaciente(nuevoPaciente);

                if (pacienteRegistrado != null)
                {
                    new Notificacion("Paciente registrado correctamente.", Notificacion.TipoNotificacion.Exito).ShowDialog();
                    this.DialogResult = true; 
                    this.Close();
                }
                else
                {
                    new Notificacion("Ocurrió un error al registrar el paciente.", Notificacion.TipoNotificacion.Error).ShowDialog();
                }
            }
            else // Modo Edición
            {
                _pacienteParaEditar.DNI = txtDni.Text.Trim();
                _pacienteParaEditar.Nombre = txtNombre.Text.Trim().ToUpper();
                _pacienteParaEditar.Apellido = txtApellido.Text.Trim().ToUpper();

                string rpta = _pacienteController.ActualizarPaciente(_pacienteParaEditar);

                if (rpta == "OK")
                {
                    new Notificacion("Paciente actualizado correctamente.", Notificacion.TipoNotificacion.Exito).ShowDialog();
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    new Notificacion(rpta, Notificacion.TipoNotificacion.Error).ShowDialog();
                }
            }
        }

        private void ResetearErrores()
        {
            errDni.Visibility = Visibility.Collapsed;
            errNombre.Visibility = Visibility.Collapsed;
            errApellido.Visibility = Visibility.Collapsed;
        }

        private bool ValidarCampos()
        {
            ResetearErrores();
            bool esValido = true;

            if (string.IsNullOrWhiteSpace(txtDni.Text))
            {
                errDni.Text = "POR FAVOR INGRESE UN DNI VÁLIDO.";
                errDni.Visibility = Visibility.Visible;
                esValido = false;
            }
            else if (!txtDni.Text.All(char.IsDigit))
            {
                errDni.Text = "EL DNI SOLO DEBE CONTENER NÚMEROS.";
                errDni.Visibility = Visibility.Visible;
                esValido = false;
            }

            // Validación de Nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errNombre.Text = "POR FAVOR INGRESE SU NOMBRE.";
                errNombre.Visibility = Visibility.Visible;
                esValido = false;
            }

            // Validación de Apellido
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                errApellido.Text = "POR FAVOR INGRESE SU APELLIDO.";
                errApellido.Visibility = Visibility.Visible;
                esValido = false;
            }

            return esValido;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) { this.Close(); }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    }
}
