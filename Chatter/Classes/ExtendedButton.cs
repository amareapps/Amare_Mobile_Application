using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Chatter.Classes
{
    public class ExtendedButton : Button
    {
        /// <summary>
        /// Bindable property for button content vertical alignment.
        /// </summary>
        public static readonly BindableProperty VerticalContentAlignmentProperty =
#pragma warning disable CS0618 // Type or member is obsolete
            BindableProperty.Create<ExtendedButton, TextAlignment>(
                p => p.VerticalContentAlignment, TextAlignment.Center);
#pragma warning restore CS0618 // Type or member is obsolete

        /// <summary>
        /// Bindable property for button content horizontal alignment.
        /// </summary>
        public static readonly BindableProperty HorizontalContentAlignmentProperty =
#pragma warning disable CS0618 // Type or member is obsolete
            BindableProperty.Create<ExtendedButton, TextAlignment>(
                p => p.HorizontalContentAlignment, TextAlignment.Center);
#pragma warning restore CS0618 // Type or member is obsolete

        /// <summary>
        /// Gets or sets the content vertical alignment.
        /// </summary>
        public TextAlignment VerticalContentAlignment
        {
            get { return this.GetValue<TextAlignment>(VerticalContentAlignmentProperty); }
            set { this.SetValue(VerticalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content horizontal alignment.
        /// </summary>
        public TextAlignment HorizontalContentAlignment
        {
            get { return this.GetValue<TextAlignment>(HorizontalContentAlignmentProperty); }
            set { this.SetValue(HorizontalContentAlignmentProperty, value); }
        }

        private T GetValue<T>(BindableProperty horizontalContentAlignmentProperty)
        {
            throw new NotImplementedException();
        }
    }
}
