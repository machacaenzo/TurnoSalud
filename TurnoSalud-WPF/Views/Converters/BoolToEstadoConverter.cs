using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Views.Converters
{
    public class BoolToEstadoConverter : IValueConverter
    {
        // Traducción de la lógica (bool) a la vista (string)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool estado)
            {
                return estado ? "Activo" : "Inactivo";
            }
            return string.Empty; // Devuelve un texto vacío si el valor no es un booleano
        }

        // No necesitamos la conversión inversa
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
