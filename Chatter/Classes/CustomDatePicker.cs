using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Chatter.Classes
{
    public class CustomDatePicker : DatePicker
    {
        public static readonly BindableProperty EnterTextProperty = BindableProperty.Create(propertyName: "Placeholder", returnType: typeof(string), declaringType: typeof(CustomDatePicker), defaultValue: default(string));
        public string Placeholder { get; set; }
    }
}