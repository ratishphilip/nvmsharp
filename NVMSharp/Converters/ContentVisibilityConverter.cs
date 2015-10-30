using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using NVMSharp.Common;

namespace NVMSharp.Converters
{
    public class ContentVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var vmValue = value.ToString();
            var parValue = parameter as string;

            var result = Visibility.Collapsed;

            if (!String.IsNullOrWhiteSpace(parValue))
            {
                var parSplitValues = parValue.Split(new string[] { Constants.ENUM_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
                {
                    var matchValue = parSplitValues.FirstOrDefault(x => x.Trim() == vmValue);
                    result = (matchValue != null) ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
