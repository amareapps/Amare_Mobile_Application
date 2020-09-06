using Chatter.Classes;
using Chatter.Model;
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
        MessageCenterManager notifier = new MessageCenterManager();
        public ChatModel VM => ((ChatModel)BindingContext);
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

        private void imagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            notifier.sendAction(VM, imagePicker.SelectedIndex);
        }

        private void MultiGestureView_LongPressed(object sender, EventArgs e)
        {
            imagePicker.Focus();
        }
        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            notifier.sendAction(VM, 0);
        }

        private void MenuItem_Clicked_1(object sender, EventArgs e)
        {
            notifier.sendAction(VM, 1);
        }

        private void MenuItem_Clicked_2(object sender, EventArgs e)
        {
            notifier.sendAction(VM, 2);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            notifier.viewImage(VM.message);
        }
    }
}