using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Chatter.Classes;
using Chatter.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using DatePicker = Xamarin.Forms.DatePicker;

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerAndroidRenderer))]

namespace Chatter.Droid
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class CustomDatePickerAndroidRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);
            this.Control.SetTextColor(Android.Graphics.Color.Rgb(0, 0, 0));
            this.Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            this.Control.SetPadding(50, 35, 50, 35);

            GradientDrawable gd = new GradientDrawable();
            gd.SetCornerRadius(30f); //increase or decrease to changes the corner look  
            gd.SetColor(Android.Graphics.Color.Transparent);
            gd.SetStroke(5, Android.Graphics.Color.Rgb(152, 0, 11));

            this.Control.SetBackgroundDrawable(gd);

            CustomDatePicker element = Element as CustomDatePicker;

            if (!string.IsNullOrWhiteSpace(element.Placeholder))
            {
                Control.Text = element.Placeholder;
            }

            this.Control.TextChanged += (sender, arg) => {
                var selectedDate = arg.Text.ToString();
                if (selectedDate == element.Placeholder)
                {
                    Control.Text = DateTime.Now.ToString("MM/dd/yyyy");
                }
            };
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}