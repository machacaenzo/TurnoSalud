using Controllers;
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

namespace Views.Paginas.EstacionAutogestion
{
    /// <summary>
    /// Lógica de interacción para AgendarTurnos.xaml
    /// </summary>
    public partial class AgendarTurnos : Page
    {
        private readonly EspecialidadController _especialidadController = new EspecialidadController();
        private readonly MedicoController _medicoController = new MedicoController();
        private readonly HorarioController _horarioController = new HorarioController();

        private readonly Paciente _paciente;
        private Medico _medicoSeleccionado;

        public AgendarTurnos(Paciente paciente)
        {
            InitializeComponent();
            _paciente = paciente;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lblNombrePaciente.Text = $"{_paciente.Nombre} {_paciente.Apellido}";
            CargarEspecialidades();
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
                new Notificaciones.Notificacion("Error al cargar especialidades: " + ex.Message, Notificaciones.Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void cmbEspecialidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEspecialidad.SelectedValue == null) return;
            try
            {
                int idEspecialidad = (int)cmbEspecialidad.SelectedValue;
                var medicos = _medicoController.ListarMedicos("").Where(m => m.IdEspecialidad == idEspecialidad).ToList();

                icMedicos.ItemsSource = medicos.Select(m => new {
                    Medico = m,
                    NombreCompleto = $"{m.Nombre} {m.Apellido}",
                    Especialidad = m.Especialidad,
                    Foto = ByteToImageSource(m.Foto) ?? new BitmapImage(new Uri("pack://application:,,,/img/user.png"))
                }).ToList();

                LimpiarSelecciones();
            }
            catch (Exception ex)
            {
                new Notificaciones.Notificacion("Error al cargar médicos: " + ex.Message, Notificaciones.Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void MedicoCard_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton)?.DataContext is object dataContext)
            {
                _medicoSeleccionado = (Medico)dataContext.GetType().GetProperty("Medico").GetValue(dataContext, null);
                CargarHorariosDisponibles();
            }
        }

        private void CargarHorariosDisponibles()
        {
            if (_medicoSeleccionado == null) return;
            try
            {
                var horariosDisponibles = new List<HorarioDetalle>();
                var fechaDesde = DateTime.Today.AddDays(1);
                for (int i = 0; i < 7; i++)
                {
                    var fechaABuscar = fechaDesde.AddDays(i);
                    var horariosDelDia = _horarioController.ListarHorariosDisponiblesDelMedico(_medicoSeleccionado.IdMedico, fechaABuscar);
                    foreach (var horario in horariosDelDia) { horario.FechaTurno = fechaABuscar; }
                    horariosDisponibles.AddRange(horariosDelDia);
                }
                dgHorariosDisponibles.ItemsSource = horariosDisponibles.OrderBy(h => h.FechaTurno).ThenBy(h => h.HoraInicio).ToList();
                dgHorariosDisponibles.Visibility = Visibility.Visible;
                lblPlaceholderHorarios.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                new Notificaciones.Notificacion("Error al cargar horarios: " + ex.Message, Notificaciones.Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void btnElegir_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is HorarioDetalle horarioElegido)
            {
                var datosTurno = new
                {
                    Paciente = _paciente,
                    Medico = _medicoSeleccionado,
                    Horario = horarioElegido
                };

                NavigationService?.Navigate(new Confirmacion(datosTurno));
            }
        }

        private void LimpiarSelecciones()
        {
            _medicoSeleccionado = null;
            dgHorariosDisponibles.Visibility = Visibility.Collapsed;
            lblPlaceholderHorarios.Visibility = Visibility.Visible;
        }

        private ImageSource ByteToImageSource(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0; image.BeginInit(); image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad; image.UriSource = null; image.StreamSource = mem; image.EndInit();
            }
            image.Freeze(); return image;
        }
    }
}
