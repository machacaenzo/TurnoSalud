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
    /// Lógica de interacción para Confirmacion.xaml
    /// </summary>
    public partial class Confirmacion : Page
    {
        private readonly TurnoController _turnoController = new TurnoController();
        private readonly Paciente _paciente;
        private readonly Medico _medico;
        private readonly HorarioDetalle _horario;

        public Confirmacion(dynamic datosTurno)
        {
            InitializeComponent();
            _paciente = datosTurno.Paciente;
            _medico = datosTurno.Medico;
            _horario = datosTurno.Horario;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Llenamos los TextBlocks con la información del turno
            lblResumenPaciente.Text = $"{_paciente.Nombre} {_paciente.Apellido}";
            lblResumenMedico.Text = $"{_medico.Nombre} {_medico.Apellido}";
            lblResumenDia.Text = _horario.DiaNombre;
            lblResumenHoraInicio.Text = _horario.HoraInicioStr;
            lblResumenHoraFin.Text = _horario.HoraFinStr;
            lblResumenFecha.Text = _horario.FechaTurno.ToString("dd/MM/yyyy");
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var nuevoTurno = new Turno
                {
                    IdPaciente = _paciente.IdPaciente,
                    IdMedico = _medico.IdMedico,
                    IdFranja = _horario.IdFranja,
                    FechaTurno = _horario.FechaTurno
                };

                int idTurno = _turnoController.RegistrarTurno(nuevoTurno);

                if (idTurno > 0)
                {
                    
                    NavigationService?.Navigate(new Comprobante(idTurno));
                }
                else
                {
                    new Notificaciones.Notificacion("No se pudo registrar el turno.", Notificaciones.Notificacion.TipoNotificacion.Error).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new Notificaciones.Notificacion("Error al confirmar el turno:\n" + ex.Message, Notificaciones.Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }
    }
}
