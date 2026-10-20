using Controllers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Views.Utilities;

namespace Views.Paginas.EstacionAutogestion
{
    /// <summary>
    /// Lógica de interacción para Comprobante.xaml
    /// </summary>
    public partial class Comprobante : Page
    {
        private readonly ReporteController _reporteController = new ReporteController();
        private Turno _turnoCompleto;
        private readonly int _idTurno;

        public Comprobante(int idTurno)
        {
            InitializeComponent();
            _idTurno = idTurno;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatosTurno();
        }

        private void CargarDatosTurno()
        {
            try
            {
                
                _turnoCompleto = _reporteController.ObtenerComprobanteTurno(_idTurno);

                if (_turnoCompleto == null)
                {
                    new Notificaciones.Notificacion("No se pudieron cargar los detalles del turno recién registrado.", Notificaciones.Notificacion.TipoNotificacion.Error).ShowDialog();
                    btnFinalizar_Click(null, null);
                    return;
                }

                // Llenamos los TextBlocks de la vista
                lblPaciente.Text = _turnoCompleto.NombrePaciente;
                lblMedico.Text = _turnoCompleto.NombreMedico;
                lblEspecialidad.Text = _turnoCompleto.Especialidad;
                lblFecha.Text = _turnoCompleto.FechaTurno.ToString("dd/MM/yyyy");
                lblHorario.Text = $"{_turnoCompleto.HoraInicioStr} - {_turnoCompleto.HoraFinStr}";
                lblBloque.Text = _turnoCompleto.Bloque;
            }
            catch (Exception ex)
            {
                new Notificaciones.Notificacion("Error al cargar los datos del comprobante: " + ex.Message, Notificaciones.Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void btnGuardarPdf_Click(object sender, RoutedEventArgs e)
        {
            if (_turnoCompleto == null) return;

            var saveFileDialog = new SaveFileDialog
            {
                FileName = $"Comprobante_Turno_{_turnoCompleto.IdTurno}_{_turnoCompleto.NombrePaciente.Replace(" ", "_")}.pdf",
                Filter = "Archivos PDF (*.pdf)|*.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ReporteGenerator.GenerarComprobanteTurnoPdf(_turnoCompleto, saveFileDialog.FileName);
                    new Notificaciones.Notificacion("PDF guardado correctamente.", Notificaciones.Notificacion.TipoNotificacion.Exito).ShowDialog();
                }
                catch (Exception ex)
                {
                    new Notificaciones.Notificacion("Error al generar el PDF: " + ex.Message, Notificaciones.Notificacion.TipoNotificacion.Error).ShowDialog();
                }
            }
        }

        private void btnMisTurnos_Click(object sender, RoutedEventArgs e)
        {
            if (_turnoCompleto != null)
            {
               
                NavigationService?.Navigate(new MisTurnos(_turnoCompleto.IdPaciente, _turnoCompleto.NombrePaciente));
            }
        }

        private void btnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            var navService = NavigationService.GetNavigationService(this);
            while (navService != null && navService.CanGoBack)
            {
                navService.RemoveBackEntry();
            }
            navService?.Navigate(new Bienvenida());
        }
    }
}
