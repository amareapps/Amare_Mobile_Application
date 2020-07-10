using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chatter;
using Chatter.Classes;
using Chatter.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(OTPEntry), typeof(OTPEntryIOSRenderer))]
namespace Chatter.iOS
{
    public class OTPEntryIOSRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            Control.TintColor = UIColor.White;

        }
    }
}