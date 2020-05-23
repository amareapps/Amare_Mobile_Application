using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View.Cells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OutgoingViewCellImage : ViewCell
    {
        public OutgoingViewCellImage()
        {
            InitializeComponent();
            imageSent.SizeChanged += ImageSent_SizeChanged;
        }

        private void ImageSent_SizeChanged(object sender, EventArgs e)
        {
            if (imageSent.Height > 350)
                imageSent.HeightRequest = 350;
            if (imageSent.Width > 300)
                imageSent.WidthRequest = 300;
            if (imageSent.Height < 200)
                imageSent.HeightRequest = 200;
            if (imageSent.Width > 150)
                imageSent.WidthRequest = 150;
        }
    }
}