using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VipPremium : ContentPage
    {
        public VipPremium()
        {
            InitializeComponent();
        }

        private void backButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void vipPremium1_Tapped(object sender, EventArgs e)
        {

        }

        private void vipPremium2_Tapped(object sender, EventArgs e)
        {

        }

        private void vipPremium3_Tapped(object sender, EventArgs e)
        {

        }
    }
}