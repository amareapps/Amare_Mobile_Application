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
    public partial class IncomingViewCellReply : ViewCell
    {
        MessageCenterManager notifier = new MessageCenterManager();
        public ChatModel VM => ((ChatModel)BindingContext);
        public IncomingViewCellReply()
        {
            InitializeComponent();
        }
        private void MultiGestureView_LongPressed(object sender, EventArgs e)
        {
            imagePicker.Focus();
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