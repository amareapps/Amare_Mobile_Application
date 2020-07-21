using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Chatter.Droid;
using Xamarin.Forms;
using Chatter.Classes;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(CustomEditorMatch), typeof(CustomEditorAndroidRendererMatch))]

namespace Chatter.Droid
{
    public class CustomEditorAndroidRendererMatch : EditorRenderer
    {
        public CustomEditorAndroidRendererMatch(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetCornerRadius(30f);
                gd.SetStroke(10, Android.Graphics.Color.Transparent);
                Control.SetAutoSizeTextTypeWithDefaults(AutoSizeTextType.Uniform);
                Control.SetMinHeight(200);
                Control.SetBackground(gd);

                Control.SetPadding(30, 20, 30, 20);
            }
        }
    }
}