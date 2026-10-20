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

namespace Views.Notificaciones
{
    /// <summary>
    /// Lógica de interacción para Notificacion.xaml
    /// </summary>
    public partial class Notificacion : Window
    {
        public enum TipoNotificacion
        {
            Exito,
            Error,
            Confirmacion
        }

        public Notificacion(string mensaje, TipoNotificacion tipo, string subtitulo = "")
        {
            InitializeComponent();

            txtMensaje.Text = mensaje;
            if (!string.IsNullOrEmpty(subtitulo))
            {
                txtSubtitulo.Text = subtitulo;
                txtSubtitulo.Visibility = Visibility.Visible;
            }

            // Aquí aplicamos los estilos según el tipo de notificación
            switch (tipo)
            {
                case TipoNotificacion.Exito:
                    HeaderBorder.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#28A745"));
                    Icono.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Check;
                    btnAceptar.Style = (Style)FindResource("SuccessButton");
                    break;

                case TipoNotificacion.Error:
                    HeaderBorder.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DC3545"));
                    Icono.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Close;
                    btnAceptar.Style = (Style)FindResource("DangerButton");
                    break;

                case TipoNotificacion.Confirmacion:
                    HeaderBorder.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC107"));
                    Icono.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Alert;
                    btnAceptar.Style = (Style)FindResource("SuccessButton");
                    btnCancelar.Style = (Style)FindResource("DangerButton");
                    btnCancelar.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

