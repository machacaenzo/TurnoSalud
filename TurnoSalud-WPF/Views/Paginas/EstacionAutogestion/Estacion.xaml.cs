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

namespace Views.Paginas.EstacionAutogestion
{
    /// <summary>
    /// Lógica de interacción para Estacion.xaml
    /// </summary>
    public partial class Estacion : Window
    {
        public Estacion()
        {
            InitializeComponent();
            MainFrame.Navigate(new Bienvenida());
        }
    }
}
