using Android.Graphics;
using Android.Graphics.Drawables;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(Droid.Effects.RangeSliderThumbImage), nameof(Droid.Effects.RangeSliderThumbImage))]

namespace Droid.Effects
{
    public class RangeSliderThumbImage : PlatformEffect
    {
        protected override void OnAttached()
        {
            var themeColor = Xamarin.Forms.Color.LightGray.ToAndroid();
            var ctrl = (Xamarin.RangeSlider.RangeSliderControl)Control;
            ctrl.ActiveColor = themeColor;

#pragma warning disable CS0618 // Type or member is obsolete
            var thumbImage = new BitmapDrawable(ctrl.ThumbImage);
#pragma warning restore CS0618 // Type or member is obsolete
            thumbImage.SetColorFilter(new PorterDuffColorFilter(themeColor, PorterDuff.Mode.SrcIn));
            ctrl.ThumbImage = ConvertToBitmap(thumbImage, ctrl.ThumbImage.Width, ctrl.ThumbImage.Height);

#pragma warning disable CS0618 // Type or member is obsolete
            var thumbPressedImage = new BitmapDrawable(ctrl.ThumbPressedImage);
#pragma warning restore CS0618 // Type or member is obsolete
            thumbPressedImage.SetColorFilter(new PorterDuffColorFilter(themeColor, PorterDuff.Mode.SrcIn));
            ctrl.ThumbPressedImage = ConvertToBitmap(thumbPressedImage, ctrl.ThumbPressedImage.Width, ctrl.ThumbPressedImage.Height);
        }

        protected override void OnDetached()
        {
        }

        private static Bitmap ConvertToBitmap(Drawable drawable, int widthPixels, int heightPixels)
        {
            var mutableBitmap = Bitmap.CreateBitmap(widthPixels, heightPixels, Bitmap.Config.Argb8888);
            var canvas = new Canvas(mutableBitmap);
            drawable.SetBounds(0, 0, widthPixels, heightPixels);
            drawable.Draw(canvas);
            return mutableBitmap;
        }
    }
}