using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NVMSharp.Converters
{
    public class GeometryPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string dataKey = value as string;
            if (String.IsNullOrWhiteSpace(dataKey))
                return null;

            

            // obtain the conveter for the target type
            //TypeConverter converter = TypeDescriptor.GetConverter(targetType);

            try
            {
                var data = Application.Current.Resources[dataKey] as string;
                return String.IsNullOrWhiteSpace(data) ? null : Geometry.Parse(data);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
