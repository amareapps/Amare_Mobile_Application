using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Chatter.Classes;
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
            location = e.Position.Latitude.ToString() + "," + e.Position.Longitude.ToString();
            userModel.location = location;
            sqliteManager.updateUserModel(userModel);
            await api.updateUser(sqliteManager.getUserModel());
            await Navigation.PopAsync();
        }
    }
}