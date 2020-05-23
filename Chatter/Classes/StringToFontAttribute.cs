using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Chatter.Classes
{
    class StringToFontAttribute : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string valuer = value.ToString();
            if(valuer == "1")
            {
                return FontAttributes.Bold;
            }
            else
            {
                return FontAttributes.None;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
