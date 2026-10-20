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
using System.Windows.Shapes;
using Views.Notificaciones;

namespace Views.Paginas.Pacientes
{
    /// <summary>
    /// Lógica de interacción para VerTurnosHoy.xaml
    /// </summary>
    public partial class VerTurnosHoy : Window
    {
        private readonly TurnoController _turnoController = new TurnoController();

        public VerTurnosHoy()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblTitulo.Text += $" ({DateTime.Today:dd/MM/yyyy})";
            CargarTurnosDelDia();
        }

        private void CargarTurnosDelDia()
        {
            try
            {
                dgTurnosHoy.ItemsSource = _turnoController.ListarTurnosDelDia();
            }
            catch (Exception ex)
            {
                new Notificacion("Error al cargar los turnos del día: " + ex.Message, Notificacion.TipoNotificacion.Error) { Owner = this }.ShowDialog();
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
