using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Chatter.Classes
{
    class NumbertoBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string valueupdate = value.ToString();
                if (valueupdate == "0")
                    return false;
                else
                    return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool valuer = (bool)value;
            if (valuer)
                return "1";
            else
                return "0";
            //throw new NotImplementedException();
        }
    }
}
