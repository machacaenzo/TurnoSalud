using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Views.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        // Este es el método que hace la traducción "hacia adelante" (de la lógica a la vista)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Verificamos si el valor que nos llega es un booleano
            if (value is bool booleanValue)
            {
                // Si es 'true', devolvemos Visible. Si es 'false', devolvemos Collapsed.
                return booleanValue ? Visibility.Visible : Visibility.Collapsed;
            }

            // Si por alguna razón el valor no es un booleano, lo ocultamos por seguridad.
            return Visibility.Collapsed;
        }

        // Este método haría la traducción "hacia atrás" (de la vista a la lógica).
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
