using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
namespace Chatter.Classes
{
    class BirthdaytoAgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] test = value.ToString().Split('/');
            string complete = test[1] + "/" + test[0] + "/" + test[2];
            var birthdate = System.Convert.ToDateTime(complete);
            // Calculate the age.
            var age = new DateTime(DateTime.Now.Subtract(birthdate).Ticks).Year - 1;
            return age;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
