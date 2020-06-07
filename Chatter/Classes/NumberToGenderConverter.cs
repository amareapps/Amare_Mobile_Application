using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Chatter.Classes
{
    class NumberToGenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string valueupdate = value.ToString();
            if (valueupdate == "0")
                return "Woman";
            else
                return "Man";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string valueupdate = value.ToString();
            if (valueupdate == "Woman")
                return "0";
            else
                return "1";
            //throw new NotImplementedException();
        }
    }
}
