using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AutoSwitchING.Converters
{
    public class SwitchTypeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SwitchType)
            {
                return SwitchTypeHelper.Names.Where(n => n.Value == (SwitchType)value).FirstOrDefault();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as SwitchTypeHelper.SwitchTypeName)?.Value;
        }
    }
}
