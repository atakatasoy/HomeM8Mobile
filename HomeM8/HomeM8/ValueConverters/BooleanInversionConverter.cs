using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HomeM8
{
    public class BooleanInversionConverter : BaseValueConverter<BooleanInversionConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
