using System;
using System.Collections.Generic;
using System.ComponentModel;
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

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonAndroidRenderer))]
namespace Chatter.Droid
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class ExtendedButtonAndroidRenderer : ButtonRenderer
    {
        /// <summary>
        /// Called when [element changed].
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            UpdateAlignment();
          //  UpdateFont();
        }

        /// <summary>
        /// Handles the <see cref="E:ElementPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ExtendedButton.VerticalContentAlignmentProperty.PropertyName ||
                e.PropertyName == ExtendedButton.HorizontalContentAlignmentProperty.PropertyName)
            {
                UpdateAlignment();
            }
            else if (e.PropertyName == Xamarin.Forms.Button.FontProperty.PropertyName)
            {
            //    UpdateFont();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        /// <summary>
        /// Updates the font
        /// </summary>
        //private void UpdateFont()
       // {
         //   Control.Typeface = Element.Font.ToExtendedTypeface(Context);
        //}

        /// <summary>
        /// Sets the alignment.
        /// </summary>
        private void UpdateAlignment()
        {
            var element = this.Element as ExtendedButton;

            if (element == null || this.Control == null)
            {
                return;
            }

          //  this.Control.Gravity = element.VerticalContentAlignment.ToDroidVerticalGravity() |
          //      element.HorizontalContentAlignment.ToDroidHorizontalGravity();
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}