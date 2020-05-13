using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfApplication1.Commons.Converters
{
    public class RadianToDegrees : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return RadianToDegree((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true;
        }

        private double RadianToDegree(double angle)
        {
            return Math.Round(angle * (180.0 / Math.PI), 2);
        }
    }
}
