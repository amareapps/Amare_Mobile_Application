using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapViewer : ContentPage
    {
        public MapViewer()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {

        }

        private void map_MapClicked(object sender, MapClickedEventArgs e)
        {
            map.Pins.Clear();
            map.Pins.Add(new Pin
            {
                Label = "Pin from tap",
                Position = new Position(e.Position.Latitude, e.Position.Longitude)
            });
        }
        }
    }