
using System;
using System.Collections.Generic;
using Chatter.Classes;
using Chatter.Model;
using Xamarin.Forms;
namespace Chatter.View.Cells
{
    public partial class IncomingViewCell : ViewCell
    {
        MessageCenterManager notifier = new MessageCenterManager();
        public ChatModel VM => ((ChatModel)BindingContext);
        public IncomingViewCell()
        {
            InitializeComponent();
        }

        private void imagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            notifier.sendAction(VM, imagePicker.SelectedIndex);
        }

        private void MultiGestureView_LongPressed(object sender, EventArgs e)
        {
            imagePicker.Focus();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            notifier.viewProfile(VM.sender_id);
            //await Navigation.PushModalAsync(new ViewProfile(VM.sender_id));
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            notifier.sendAction(VM,0);
        }

        private void MenuItem_Clicked_1(object sender, EventArgs e)
        {
            notifier.sendAction(VM, 1);
        }

        private void MenuItem_Clicked_2(object sender, EventArgs e)
        {
            notifier.sendAction(VM, 2);
        }
    }
}