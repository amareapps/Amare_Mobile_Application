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

[assembly: ExportRenderer(typeof(SearchBarNoUnderline), typeof(SearchBarNoUnderlineRenderer))]

namespace Chatter.Droid
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class SearchBarNoUnderlineRenderer : SearchBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var plateId = Resources.GetIdentifier("android:id/search_plate", null, null);
                var plate = Control.FindViewById(plateId);
                plate.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }

            var searchView = (Control as SearchView);
            var searchIconId = searchView.Resources.GetIdentifier("android:id/search_mag_icon", null, null);
            if (searchIconId > 0)
            {
                var searchPlateIcon = searchView.FindViewById(searchIconId);
                (searchPlateIcon as ImageView).SetImageResource(Resource.Drawable.searchicon);

            }
        }

    }
#pragma warning restore CS0618 // Type or member is obsolete
}