using Controllers;
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
using Views.Notificaciones;

namespace Views.Paginas.Reportes
{
    /// <summary>
    /// Lógica de interacción para Reportes.xaml
    /// </summary>
    public partial class Reportes : UserControl
    {
        private readonly MedicoController _medicoController = new MedicoController();
        private readonly ReporteController _reporteController = new ReporteController();

        public Reportes()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var hoy = DateTime.Today;
            var primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1);
            dpFechaDesde1.SelectedDate = primerDiaMes; dpFechaHasta1.SelectedDate = hoy;
            dpFechaDesde2.SelectedDate = primerDiaMes; dpFechaHasta2.SelectedDate = hoy;
            dpFechaDesde3.SelectedDate = primerDiaMes; dpFechaHasta3.SelectedDate = hoy;
            dpFechaDesde4.SelectedDate = primerDiaMes; dpFechaHasta4.SelectedDate = hoy;
            CargarMedicos();
        }

        private void CargarMedicos()
        {
            try
            {
                var medicos = _medicoController.ListarMedicos("");
                var medicosConNombre = medicos.Select(m => new { m.IdMedico, NombreCompleto = $"{m.Nombre} {m.Apellido}" }).ToList();
                cmbMedico.ItemsSource = medicosConNombre;
                cmbMedico.DisplayMemberPath = "NombreCompleto";
                cmbMedico.SelectedValuePath = "IdMedico";
            }
            catch (Exception ex) {  }
        }

        private void btnGenerarTurnosPorFecha_Click(object sender, RoutedEventArgs e)
        {
            var datos = _reporteController.ReporteTurnosPorFecha(dpFechaDesde1.SelectedDate.Value, dpFechaHasta1.SelectedDate.Value);
            var ventana = new VisorReportes("Reporte de Turnos por Fecha", datos);
            ventana.Owner = Window.GetWindow(this);
            ventana.ShowDialog();
        }

        private void btnGenerarTurnosPorMedico_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMedico.SelectedValue == null) { /* ... */ return; }
            int idMedico = (int)cmbMedico.SelectedValue;
            var datos = _reporteController.ReporteTurnosPorMedico(idMedico, dpFechaDesde2.SelectedDate.Value, dpFechaHasta2.SelectedDate.Value);
            var ventana = new VisorReportes("Reporte de Turnos para " + cmbMedico.Text, datos);
            ventana.Owner = Window.GetWindow(this);
            ventana.ShowDialog();
        }

        private void btnGenerarTurnosPorEspecialidad_Click(object sender, RoutedEventArgs e)
        {
            var datos = _reporteController.ReporteTurnosPorEspecialidad(dpFechaDesde3.SelectedDate.Value, dpFechaHasta3.SelectedDate.Value);
            var ventana = new VisorReportes("Reporte de Turnos por Especialidad", datos);
            ventana.Owner = Window.GetWindow(this);
            ventana.ShowDialog();
        }

        private void btnGenerarPacientesAtendidos_Click(object sender, RoutedEventArgs e)
        {
            var datos = _reporteController.ReportePacientesAtendidos(dpFechaDesde4.SelectedDate.Value, dpFechaHasta4.SelectedDate.Value);
            var ventana = new VisorReportes("Reporte de Pacientes Atendidos", datos);
            ventana.Owner = Window.GetWindow(this);
            ventana.ShowDialog();
        }
    }
}
