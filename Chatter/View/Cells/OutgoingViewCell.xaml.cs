using Android.OS;
using Chatter.Classes;
using Chatter.Model;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Chatter.View.Cells
{
    public partial class OutgoingViewCell : ViewCell
    {
        public ChatModel VM => ((ChatModel)BindingContext);
        MessageCenterManager notifier = new MessageCenterManager();
        public OutgoingViewCell()
        {
            InitializeComponent();
        }

        private void MultiGestureView_LongPressed(object sender, EventArgs e)
        {

            //MessagingCenter.Send<OutgoingViewCell>(this, "Hi");
            imagePicker.Focus();
            //IRefreshInbox logger = new Messaging("","","","","");
            //ListManager managers = new ListManager(logger);
            //managers.RefreshInbox();
            //ss.LongPressEffect_LongPressed(null,null);
            //await Application.Current.MainPage.DisplayAlert("Ay Barbie","Sabi ko","na");
        }

        private void imagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            notifier.sendAction(VM, imagePicker.SelectedIndex);
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