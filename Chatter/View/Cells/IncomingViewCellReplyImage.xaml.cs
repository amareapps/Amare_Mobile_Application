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
    public partial class IncomingViewCellReplyImage : ViewCell
    {
        public IncomingViewCellReplyImage()
        {
            InitializeComponent();
        }

        private void imagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void MultiGestureView_LongPressed(object sender, EventArgs e)
        {

        }
    }
}