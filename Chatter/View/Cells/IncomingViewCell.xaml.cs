
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
    }
}