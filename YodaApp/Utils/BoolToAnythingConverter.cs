using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace YodaApp.Utils
{
    public enum BoolToAnythingDefault
    {
        Null,
        TrueValue,
        FalseValue
    }

    public class BoolToAnythingConverter : IValueConverter
    {
        public object FalseValue { get; set; }

        public object TrueValue { get; set; }

        public bool DefaultBool { get; set; } = false;

        public object FallbackValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? TrueValue : FalseValue;

            return FallbackValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == TrueValue)
                return true;

            if (value == FalseValue)
                return false;

            return DefaultBool;
        }
    }
}
