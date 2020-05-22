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

[assembly: ExportRenderer(typeof(CustomView), typeof(CustomPressRenderer))]
namespace Chatter.Droid
{
    [Obsolete]
    public class CustomPressRenderer : ViewRenderer<CustomView, Android.Views.View>
    {
        private CustomViewListener _listener;
        private GestureDetector _detector;

        public CustomViewListener Listener
        {
            get
            {
                return _listener;
            }
        }

        public GestureDetector Detector
        {
            get
            {
                return _detector;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CustomView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                GenericMotion += HandleGenericMotion;
                Touch += HandleTouch;

                _listener = new CustomViewListener(Element);
                _detector = new GestureDetector(_listener);
            }
        }

        protected override void Dispose(bool disposing)
        {
            GenericMotion -= HandleGenericMotion;
            Touch -= HandleTouch;

            _listener = null;
            _detector?.Dispose();
            _detector = null;

            base.Dispose(disposing);
        }

        void HandleTouch(object sender, TouchEventArgs e)
        {
            _detector.OnTouchEvent(e.Event);
        }

        void HandleGenericMotion(object sender, GenericMotionEventArgs e)
        {
            _detector.OnTouchEvent(e.Event);
        }
    }

    public class CustomViewListener : GestureDetector.SimpleOnGestureListener
    {
        readonly CustomView _target;

        public CustomViewListener(CustomView s)
        {
            _target = s;
        }

        public override void OnLongPress(MotionEvent e)
        {
            _target.RaiseLongPressEvent();

            base.OnLongPress(e);
        }
    }
}