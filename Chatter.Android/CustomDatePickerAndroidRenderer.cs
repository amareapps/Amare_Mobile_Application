using System;
using Android.Graphics.Drawables;
using Chatter.Classes;
using Chatter.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerAndroidRenderer))]

namespace Chatter.Droid
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class CustomDatePickerAndroidRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);


            GradientDrawable gd = new GradientDrawable();
            gd.SetCornerRadius(30f); //increase or decrease to changes the corner look
            gd.SetColor(Android.Graphics.Color.Transparent);
            gd.SetStroke(5, Android.Graphics.Color.ParseColor("#310881"));

            this.Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            this.Control.SetPadding(50, 30, 50, 30);
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