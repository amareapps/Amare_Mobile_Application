using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Chatter.Classes
{
    class MessageContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string passedValue = (string)value;
            if (passedValue.Contains("chatter-7b8e4") && passedValue.Contains("UserImages"))
            {
                return "sent an image";
            }
            if (passedValue.Contains("chatter-7b8e4") && passedValue.Contains("AudioCollection"))
            {
                return "sent a voice clip";
            }
            if (passedValue.Contains("firebasestorage"))
            {
                return "sent an attachment";
            }
            return passedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
