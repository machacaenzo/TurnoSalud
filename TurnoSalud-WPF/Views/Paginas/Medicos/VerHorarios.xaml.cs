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

namespace Views.Paginas.Medicos
{
    /// <summary>
    /// Lógica de interacción para VerHorarios.xaml
    /// </summary>
    public partial class VerHorarios : Window
    {
        private readonly HorarioController _horarioController = new HorarioController();
        private readonly Medico _medicoSeleccionado;

        public VerHorarios(Medico medico)
        {
            InitializeComponent();
            _medicoSeleccionado = medico;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblMedicoNombre.Text += $"{_medicoSeleccionado.Nombre.ToUpper()} {_medicoSeleccionado.Apellido.ToUpper()}";
            CargarHorarios();
        }

        private void CargarHorarios()
        {
            try
            {
                dgHorarios.ItemsSource = _horarioController.ListarHorariosDeMedico(_medicoSeleccionado.IdMedico);
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar los horarios del médico: " + ex.Message, Notificacion.TipoNotificacion.Error).ShowDialog();
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is HorarioDetalle horarioParaEliminar)
            {
                var confirmacion = new Notificacion($"¿Está seguro de eliminar el horario de {horarioParaEliminar.HoraInicioStr} a {horarioParaEliminar.HoraFinStr} del día {horarioParaEliminar.DiaNombre}?", Notificacion.TipoNotificacion.Confirmacion);

                if (confirmacion.ShowDialog() == true)
                {
                    string rpta = _horarioController.DarBajaHorarioMedico(
                        _medicoSeleccionado.IdMedico,
                        horarioParaEliminar.IdFranja,
                        horarioParaEliminar.DiaSemana
                    );

                    if (rpta == "OK")
                    {
                        new Notificacion("Horario eliminado correctamente.", Notificacion.TipoNotificacion.Exito).ShowDialog();
                        CargarHorarios();
                    }
                    else
                    {
                        new Notificacion(rpta, Notificacion.TipoNotificacion.Error).ShowDialog();
                    }
                }
            }
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
