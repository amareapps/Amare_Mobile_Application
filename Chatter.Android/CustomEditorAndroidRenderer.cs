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

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorAndroidRenderer))]

namespace Chatter.Droid
{
    public class CustomEditorAndroidRenderer : EditorRenderer
    {
        public CustomEditorAndroidRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetCornerRadius(30f);
                gd.SetStroke(5, Android.Graphics.Color.ParseColor("#3cc5d5"));
                Control.SetAutoSizeTextTypeWithDefaults(AutoSizeTextType.Uniform);
                Control.SetMinHeight(200);
                Control.SetBackground(gd);

                Control.SetPadding(30, 20, 30, 20);

                var nativeEditText = (global::Android.Widget.EditText)Control;

                //While scrolling inside Editor stop scrolling parent view.
                nativeEditText.OverScrollMode = OverScrollMode.Always;
                nativeEditText.ScrollBarStyle = ScrollbarStyles.InsideInset;
                nativeEditText.SetOnTouchListener(new DroidTouchListener());

                //For Scrolling in Editor innner area
                Control.VerticalScrollBarEnabled = true;
                Control.ScrollBarStyle = Android.Views.ScrollbarStyles.InsideInset;
                //Force scrollbars to be displayed
                Android.Content.Res.TypedArray a = Control.Context.Theme.ObtainStyledAttributes(new int[0]);
                InitializeScrollbars(a);
                a.Recycle();
            }
        }
    }
}