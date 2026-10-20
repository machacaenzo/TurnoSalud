using Entities;
using Microsoft.Win32;
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
using Views.Utilities;

namespace Views.Paginas.Pacientes
{
    /// <summary>
    /// Lógica de interacción para VisorComprobanteTurno.xaml
    /// </summary>
    public partial class VisorComprobanteTurno : Window
    {
        private readonly Turno _turnoCompleto;

        public VisorComprobanteTurno(Turno turno)
        {
            InitializeComponent();
            _turnoCompleto = turno;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Llenamos la vista con los datos del turno que recibimos
            lblPaciente.Text = _turnoCompleto.NombrePaciente;
            lblMedico.Text = _turnoCompleto.NombreMedico;
            lblEspecialidad.Text = _turnoCompleto.Especialidad;
            lblFecha.Text = _turnoCompleto.FechaTurno.ToString("dd/MM/yyyy");
            lblHorario.Text = $"{_turnoCompleto.HoraInicioStr} - {_turnoCompleto.HoraFinStr}";
            lblBloque.Text = _turnoCompleto.Bloque;
        }

        private void btnGuardarPdf_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = $"Comprobante_Turno_{_turnoCompleto.IdTurno}_{_turnoCompleto.NombrePaciente.Replace(" ", "_")}.pdf",
                Filter = "Archivos PDF (*.pdf)|*.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Llamamos a tu clase existente para generar el PDF
                    ReporteGenerator.GenerarComprobanteTurnoPdf(_turnoCompleto, saveFileDialog.FileName);
                    new Notificacion("Comprobante en PDF guardado correctamente.", Notificacion.TipoNotificacion.Exito) { Owner = this }.ShowDialog();
                }
                catch (Exception ex)
                {
                    new Notificacion("Error al generar el comprobante en PDF: " + ex.Message, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) => this.Close();
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}
