using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Locations;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Android;
using Xamarin.Forms;
using Chatter.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(LocationZ))]
namespace Chatter.Droid
{
    using System.Runtime.Remoting.Messaging;

    using Android.Support.V4.View;
    using Android.Support.V7.App;
    using Chatter.Classes;
    public class LocationZ : ILocSettings
    {
        [Obsolete]
        public bool OpenSettings()
        {
            LocationManager LM = (LocationManager)Forms.Context.GetSystemService(Context.LocationService);
            if (LM.IsProviderEnabled(LocationManager.GpsProvider) == false)
            {
                Context ctx = Forms.Context;
                ctx.StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                return true;
            }
            else
            {
                return false;
                //this is handled in the PCL
            }
        }
    }
}