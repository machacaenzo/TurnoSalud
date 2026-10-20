using Controllers;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para GestionHorario.xaml
    /// </summary>
    public partial class GestionHorario : Window
    {
        private readonly HorarioController _controller = new HorarioController();
        private readonly Medico _medicoSeleccionado;
        // Usamos ObservableCollection para que el DataGrid de la derecha se actualice automáticamente
        private readonly ObservableCollection<HorarioDetalle> _horariosParaAsignar = new ObservableCollection<HorarioDetalle>();
        private List<HorarioDetalle> _horariosExistentes = new List<HorarioDetalle>();


        public GestionHorario(Medico medico)
        {
            InitializeComponent();
            _medicoSeleccionado = medico;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblTitulo.Text += $"{_medicoSeleccionado.Nombre.ToUpper()} {_medicoSeleccionado.Apellido.ToUpper()}";
            dgHorariosAsignados.ItemsSource = _horariosParaAsignar;
            CargarHorariosExistentes();
            CargarBloques();
            CargarDiasSemana();
            CargarFranjas();
        }
        private void CargarHorariosExistentes()
        {
            try
            {
                _horariosExistentes = _controller.ListarHorariosDeMedico(_medicoSeleccionado.IdMedico);
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar los horarios existentes del médico: " + ex.Message, Notificacion.TipoNotificacion.Error).ShowDialog();
                _horariosExistentes = new List<HorarioDetalle>(); 
            }
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is FranjaHoraria franja &&
                cmbDiaSemana.SelectedValue != null)
            {
                int diaSemana = (int)cmbDiaSemana.SelectedValue;

                bool yaEnAsignacion = _horariosParaAsignar.Any(h => h.IdFranja == franja.IdFranja && h.DiaSemana == diaSemana);
                if (yaEnAsignacion)
                {
                    new Notificacion("Esta franja ya está en la lista de asignación.", Notificacion.TipoNotificacion.Error).ShowDialog();
                    return;
                }

                bool yaExisteEnBD = _horariosExistentes.Any(h => h.IdFranja == franja.IdFranja && h.DiaSemana == diaSemana);
                if (yaExisteEnBD)
                {
                    new Notificacion("Este médico ya tiene este horario asignado y activo.", Notificacion.TipoNotificacion.Error).ShowDialog();
                    return;
                }

                _horariosParaAsignar.Add(new HorarioDetalle
                {
                    IdFranja = franja.IdFranja,
                    DiaSemana = diaSemana,
                    HoraInicio = franja.HoraInicio,
                    HoraFin = franja.HoraFin
                });
            }
        }
        private void CargarBloques()
        {
            try
            {
                cmbBloques.ItemsSource = _controller.ListarBloquesHorario();
                cmbBloques.DisplayMemberPath = "Nombre";
                cmbBloques.SelectedValuePath = "IdBloque";
                if (cmbBloques.Items.Count > 0)
                {
                    cmbBloques.SelectedIndex = 0; 
                }
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar los turnos: " + ex.Message, Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void CargarDiasSemana()
        {
            var dias = new Dictionary<int, string>
            {
                { 1, "Lunes" }, { 2, "Martes" }, { 3, "Miércoles" },
                { 4, "Jueves" }, { 5, "Viernes" }, { 6, "Sábado" }, { 7, "Domingo" }
            };
            cmbDiaSemana.ItemsSource = dias;
            cmbDiaSemana.DisplayMemberPath = "Value";
            cmbDiaSemana.SelectedValuePath = "Key";
            if (cmbDiaSemana.Items.Count > 0)
            {
                cmbDiaSemana.SelectedIndex = 0; 
            }
        }

        private void CargarFranjas()
        {
            if (cmbBloques.SelectedValue == null) return;
            try
            {
                int idBloque = (int)cmbBloques.SelectedValue;
                dgFranjasDisponibles.ItemsSource = _controller.ListarFranjasPorBloque(idBloque);
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar las franjas horarias: " + ex.Message, Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void cmbBloques_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarFranjas();
        }

      
        private void btnEliminarAsignado_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is HorarioDetalle horarioParaEliminar)
            {
                _horariosParaAsignar.Remove(horarioParaEliminar);
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (_horariosParaAsignar.Count == 0)
            {
                new Notificacion("Debe agregar al menos un horario en la lista antes de guardar.", Notificacion.TipoNotificacion.Error, "Lista Vacía").ShowDialog();
                return;
            }

            var listaParaGuardar = _horariosParaAsignar.Select(h => new FranjaAsignacion
            {
                IdFranja = h.IdFranja,
                DiaSemana = h.DiaSemana
            }).ToList();

            string rpta = _controller.AsignarHorariosMedico(_medicoSeleccionado.IdMedico, listaParaGuardar);

            if (rpta == "OK")
            {
                new Notificacion("Horarios asignados correctamente.", Notificacion.TipoNotificacion.Exito).ShowDialog();
                this.DialogResult = true; 
                this.Close();
            }
            else
            {
                new Notificacion(rpta, Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) { this.Close(); }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    }
}

