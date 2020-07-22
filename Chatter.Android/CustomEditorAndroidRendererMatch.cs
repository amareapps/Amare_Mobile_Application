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
using Android.Text.Method;

[assembly: ExportRenderer(typeof(CustomEditorMatch), typeof(CustomEditorAndroidRendererMatch))]

namespace Chatter.Droid
{
    public class CustomEditorAndroidRendererMatch : EditorRenderer
    {
        public CustomEditorAndroidRendererMatch(Context context) : base(context)
        {
            AutoPackage = false;
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetCornerRadius(30f);
                gd.SetStroke(10, Android.Graphics.Color.LightGray);
                gd.SetColor(Android.Graphics.Color.LightGray);
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

    public class DroidTouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
    {
        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            v.Parent?.RequestDisallowInterceptTouchEvent(true);
            if ((e.Action & MotionEventActions.Up) != 0 && (e.ActionMasked & MotionEventActions.Up) != 0)
            {
                v.Parent?.RequestDisallowInterceptTouchEvent(false);
            }
            return false;
        }
    }
}