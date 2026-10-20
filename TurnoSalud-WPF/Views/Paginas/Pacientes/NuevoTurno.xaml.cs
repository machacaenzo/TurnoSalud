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
  
    public partial class NuevoTurno : Window
    {
        private readonly HorarioController _horarioController = new HorarioController();
        private readonly EspecialidadController _especialidadController = new EspecialidadController();
        private readonly MedicoController _medicoController = new MedicoController();
        private readonly TurnoController _turnoController = new TurnoController();

        private readonly Paciente _paciente;
        private HorarioDetalle _horarioSeleccionado;

        private readonly bool _esNuevo = true;
        private readonly Turno _turnoParaEditar;

        public NuevoTurno(Paciente paciente)
        {
            InitializeComponent();
            _paciente = paciente;
            _esNuevo = true;
        }

        public NuevoTurno(Paciente paciente, Turno turnoAEditar)
        {
            InitializeComponent();
            _paciente = paciente;
            _turnoParaEditar = turnoAEditar;
            _esNuevo = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string nombreCompletoPaciente = $"{_paciente.Nombre.ToUpper()} {_paciente.Apellido.ToUpper()}";

            lblTitulo.Text += nombreCompletoPaciente;

            lblDniPaciente.Text += _paciente.DNI;
            lblResumenPaciente.Text = $"{_paciente.Nombre} {_paciente.Apellido}";

            CargarEspecialidades();

            if (!_esNuevo)
            {
                lblTitulo.Text = "REPROGRAMAR TURNO PARA " + nombreCompletoPaciente;
                btnConfirmar.Content = "ACTUALIZAR TURNO";

                var notificacion = new Notificacion("Modo Reprogramación: Por favor, seleccione una nueva especialidad, médico y horario.", Notificacion.TipoNotificacion.Exito);
                notificacion.Owner = this;
                notificacion.ShowDialog();
            }
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
                new Notificacion("Error al cargar especialidades: " + ex.Message, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
            }
        }

        private void cmbEspecialidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEspecialidad.SelectedValue == null) return;
            try
            {
                int idEspecialidad = (int)cmbEspecialidad.SelectedValue;
                var medicos = _medicoController.ListarMedicos("").Where(m => m.IdEspecialidad == idEspecialidad).ToList();
                var medicosConNombreCompleto = medicos.Select(m => new { m.IdMedico, NombreCompleto = m.Nombre + " " + m.Apellido }).ToList();
                cmbMedico.ItemsSource = medicosConNombreCompleto;
                cmbMedico.DisplayMemberPath = "NombreCompleto";
                cmbMedico.SelectedValuePath = "IdMedico";
                dgHorariosDisponibles.ItemsSource = null;
                OcultarResumen();
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar médicos: " + ex.Message, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
            }
        }

        private void cmbMedico_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbMedico.SelectedValue == null) return;
            try
            {
                int idMedico = (int)cmbMedico.SelectedValue;
                var horariosDisponibles = new List<HorarioDetalle>();
                var fechaDesde = DateTime.Today.AddDays(1);
                for (int i = 0; i < 7; i++)
                {
                    var fechaABuscar = fechaDesde.AddDays(i);
                    var horariosDelDia = _horarioController.ListarHorariosDisponiblesDelMedico(idMedico, fechaABuscar);
                    if (horariosDelDia.Any())
                    {
                        foreach (var horario in horariosDelDia) { horario.FechaTurno = fechaABuscar; }
                        horariosDisponibles.AddRange(horariosDelDia);
                    }
                }
                dgHorariosDisponibles.ItemsSource = horariosDisponibles.OrderBy(h => h.FechaTurno).ThenBy(h => h.HoraInicio).ToList();
                OcultarResumen();
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar horarios: " + ex.Message, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
            }
        }

        private void btnElegir_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is HorarioDetalle horario)
            {
                _horarioSeleccionado = horario;
                lblResumenMedico.Text = cmbMedico.Text;
                lblResumenDia.Text = horario.DiaNombre;
                lblResumenHoraInicio.Text = horario.HoraInicioStr;
                lblResumenHoraFin.Text = horario.HoraFinStr;
                lblResumenFecha.Text = horario.FechaTurno.ToString("dd/MM/yyyy");
                pnlResumen.Visibility = Visibility.Visible;
            }
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (_horarioSeleccionado == null)
            {
                new Notificacion("Por favor, elija un nuevo horario de la lista.", Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
                return;
            }

            try
            {
                if (_esNuevo)
                {
                    var nuevoTurno = new Turno
                    {
                        IdPaciente = _paciente.IdPaciente,
                        IdMedico = (int)cmbMedico.SelectedValue,
                        IdFranja = _horarioSeleccionado.IdFranja,
                        FechaTurno = _horarioSeleccionado.FechaTurno
                    };
                    int idTurno = _turnoController.RegistrarTurno(nuevoTurno);
                    if (idTurno > 0)
                    {
                        new Notificacion("Turno registrado correctamente.", Notificacion.TipoNotificacion.Exito) { Owner = this }.ShowDialog();
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                else
                {
                    var turnoActualizado = new Turno
                    {
                        IdTurno = _turnoParaEditar.IdTurno,
                        IdMedico = (int)cmbMedico.SelectedValue,
                        IdFranja = _horarioSeleccionado.IdFranja,
                        FechaTurno = _horarioSeleccionado.FechaTurno
                    };
                    string rpta = _turnoController.ModificarTurno(turnoActualizado);
                    if (rpta == "OK")
                    {
                        new Notificacion("Turno reprogramado correctamente.", Notificacion.TipoNotificacion.Exito) { Owner = this }.ShowDialog();
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        new Notificacion(rpta, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                new Notificacion("Error al confirmar el turno:\n" + ex.Message, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
            }
        }

        private void OcultarResumen()
        {
            pnlResumen.Visibility = Visibility.Collapsed;
            _horarioSeleccionado = null;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}

