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

namespace Views.Paginas.EstacionAutogestion
{
    /// <summary>
    /// Lógica de interacción para Identificacion.xaml
    /// </summary>
    public partial class Identificacion : Page
    {
        private readonly PacienteController _pacienteController = new PacienteController();
        private bool _esNuevoPaciente = false;

        public Identificacion()
        {
            InitializeComponent();
            this.Loaded += (s, e) => txtDni.Focus();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarDni()) return;

            string dni = txtDni.Text.Trim();

            if (!_esNuevoPaciente) 
            {
                Paciente pacienteExistente = _pacienteController.BuscarPacientePorDNI(dni);

                if (pacienteExistente != null)
                {
                    NavegarSiguientePantalla(pacienteExistente);
                }
                else
                {
                    _esNuevoPaciente = true;
                    pnlNuevosDatos.Visibility = Visibility.Visible;
                    txtNombre.Focus();
                }
            }
            else 
            {
                if (!ValidarDatosNombreApellido()) return;

                var nuevoPaciente = new Paciente
                {
                    DNI = dni,
                    Nombre = txtNombre.Text.Trim().ToUpper(),
                    Apellido = txtApellido.Text.Trim().ToUpper()
                };

                Paciente pacienteRegistrado = _pacienteController.RegistrarPaciente(nuevoPaciente);

                if (pacienteRegistrado != null)
                {
                    NavegarSiguientePantalla(pacienteRegistrado);
                }
                else
                {
                    new Notificaciones.Notificacion("Ocurrió un error al registrar el paciente.", Notificaciones.Notificacion.TipoNotificacion.Error).ShowDialog();
                }
            }
        }

        private void NavegarSiguientePantalla(Paciente paciente)
        {
            NavigationService?.Navigate(new AgendarTurnos(paciente));
        }

        private void txtDni_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_esNuevoPaciente)
            {
                _esNuevoPaciente = false;
                pnlNuevosDatos.Visibility = Visibility.Collapsed;
                txtNombre.Clear();
                txtApellido.Clear();
                errNombre.Visibility = Visibility.Collapsed;
                errApellido.Visibility = Visibility.Collapsed;
            }
        }

        private bool ValidarDni()
        {
            errDni.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(txtDni.Text))
            {
                errDni.Text = "POR FAVOR, INGRESE SU DNI.";
                errDni.Visibility = Visibility.Visible;
                return false;
            }
            if (!txtDni.Text.All(char.IsDigit))
            {
                errDni.Text = "EL DNI SOLO DEBE CONTENER NÚMEROS.";
                errDni.Visibility = Visibility.Visible;
                return false;
            }
            return true;
        }

        private bool ValidarDatosNombreApellido()
        {
            errNombre.Visibility = Visibility.Collapsed;
            errApellido.Visibility = Visibility.Collapsed;
            bool esValido = true;

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errNombre.Text = "POR FAVOR, INGRESE SU NOMBRE.";
                errNombre.Visibility = Visibility.Visible;
                esValido = false;
            }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                errApellido.Text = "POR FAVOR, INGRESE SU APELLIDO.";
                errApellido.Visibility = Visibility.Visible;
                esValido = false;
            }
            return esValido;
        }
    }
}
