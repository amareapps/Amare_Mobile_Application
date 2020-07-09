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
using Chatter.Classes;
using Chatter.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(OTPEntry), typeof(OTPEntryAndroidRenderer))]

namespace Chatter.Droid
{
    public class OTPEntryAndroidRenderer : EditorRenderer
    {
        public OTPEntryAndroidRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            Control.SetCursorVisible(false);
            Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            Control.SetTextColor(Android.Graphics.Color.Transparent);
        }
    }
}