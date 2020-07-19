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
            var birthdate = DateTime.ParseExact(complete, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var datenow = DateTime.Now.ToString("dd/MM/yyyy");
            var convertsample = DateTime.ParseExact(datenow, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            //return value+ "  -  " + datenow;
            // Calculate the age.
            var age = new DateTime(convertsample.Subtract(birthdate).Ticks).Year - 1;
            return age;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
