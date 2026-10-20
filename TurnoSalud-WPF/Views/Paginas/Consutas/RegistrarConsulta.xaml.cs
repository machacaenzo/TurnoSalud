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

namespace Views.Paginas.Consutas
{
    /// <summary>
    /// Lógica de interacción para RegistrarConsulta.xaml
    /// </summary>
    public partial class RegistrarConsulta : Window
    {
        private readonly HistorialController _historialController = new HistorialController();
        private readonly int _idTurno;

        public RegistrarConsulta(Turno turno)
        {
            InitializeComponent();
            _idTurno = turno.IdTurno;
            lblTitulo.Text += $" (Paciente: {turno.NombrePaciente})";
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                var nuevoHistorial = new Historial
                {
                    IdTurno = _idTurno,
                    Sintomas = txtSintomas.Text.Trim().ToUpper(),
                    Diagnostico = txtDiagnostico.Text.Trim().ToUpper(),
                    Tratamiento = txtTratamiento.Text.Trim().ToUpper(),
                    Observaciones = txtObservaciones.Text.Trim().ToUpper()
                };

                string rpta = _historialController.RegistrarHistorial(nuevoHistorial);

                if (rpta == "OK")
                {
                    new Notificacion("Consulta registrada y turno marcado como 'Atendido'.", Notificacion.TipoNotificacion.Exito) { Owner = this }.ShowDialog();
                    this.DialogResult = true; 
                    this.Close();
                }
                else
                {
                    new Notificacion(rpta, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new Notificacion("Error inesperado al guardar la consulta: " + ex.Message, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
            }
        }

        private void ResetearErrores()
        {
            errSintomas.Visibility = Visibility.Collapsed;
            errDiagnostico.Visibility = Visibility.Collapsed;
            errTratamiento.Visibility = Visibility.Collapsed;
            errObservaciones.Visibility = Visibility.Collapsed;
        }

        private bool ValidarCampos()
        {
            ResetearErrores();
            bool esValido = true;

            if (string.IsNullOrWhiteSpace(txtSintomas.Text))
            {
                errSintomas.Text = "Los síntomas son obligatorios.";
                errSintomas.Visibility = Visibility.Visible;
                esValido = false;
            }
            if (string.IsNullOrWhiteSpace(txtDiagnostico.Text))
            {
                errDiagnostico.Text = "El diagnóstico es obligatorio.";
                errDiagnostico.Visibility = Visibility.Visible;
                esValido = false;
            }
            if (string.IsNullOrWhiteSpace(txtTratamiento.Text))
            {
                errTratamiento.Text = "El tratamiento es obligatorio.";
                errTratamiento.Visibility = Visibility.Visible;
                esValido = false;
            }
           
            return esValido;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) { this.Close(); }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    }
}
