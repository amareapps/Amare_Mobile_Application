using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Chatter.Classes;
using Xamarin.Essentials;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class MapViewer : ContentPage
    {
        ApiConnector api = new ApiConnector();
        SqliteManager sqliteManager = new SqliteManager();
        string[] locationPin;
        public MapViewer(string location)
        {
            InitializeComponent();
            locationPin = location.Split(',');
        }
        protected override void OnAppearing()
        {
            Position position = new Position(Convert.ToDouble(locationPin[0]), Convert.ToDouble(locationPin[1]));
            map.MoveToRegion(new MapSpan(position, 0.01, 0.01));
            map.Pins.Add(new Pin
            {
                Label = "Pin from tap",
                Position = new Position(position.Latitude,position.Longitude)
            });
        }

        private async void map_MapClicked(object sender, MapClickedEventArgs e)
        {
            string location;
            map.Pins.Clear();
            map.Pins.Add(new Pin
            {
                Label = "Pin from tap",
                Position = new Position(e.Position.Latitude, e.Position.Longitude)
            });
            var isAccepted = await DisplayAlert("Select Location","Are you sure to this location?","Yes","No");
            if (!isAccepted)
                return;
            //Update Location
            var userModel = sqliteManager.getUserModel();
            var userCity = await getAddress(e.Position.Latitude,e.Position.Longitude);
            location = e.Position.Latitude.ToString() + "," + e.Position.Longitude.ToString();
            userModel.location = location;
            userModel.city = userCity;
            sqliteManager.updateUserModel(userModel);
            //await DisplayAlert("Address",await getAddress(e.Position.Latitude,e.Position.Longitude),"Okay");
            await api.updateUser(sqliteManager.getUserModel());
            await Navigation.PopAsync();
        }
        private async Task<string> getAddress(double lat,double lon)
        {
            //Geocoder geoCoder = new Geocoder();
            //Position position = new Position(lat,longh);
            //IEnumerable<string> possibleAddresses = await geoCoder.GetAddressesForPositionAsync(position);
            //string address = possibleAddresses.FirstOrDefault();
            //return address;

            var placemarks = await Geocoding.GetPlacemarksAsync(lat, lon);

            var placemark = placemarks?.FirstOrDefault();
            string address = "";
            if (placemark != null)
            {
                var geocodeAddress =
                    $"AdminArea:       {placemark.AdminArea}\n" +
                    $"CountryCode:     {placemark.CountryCode}\n" +
                    $"CountryName:     {placemark.CountryName}\n" +
                    $"FeatureName:     {placemark.FeatureName}\n" +
                    $"Locality:        {placemark.Locality}\n" +
                    $"PostalCode:      {placemark.PostalCode}\n" +
                    $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                    $"SubLocality:     {placemark.SubLocality}\n" +
                    $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                    $"Thoroughfare:    {placemark.Thoroughfare}\n";
                address = placemark.Locality;
            }
            return address;
        }
    }
}