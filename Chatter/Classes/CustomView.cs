using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace Chatter.Classes
{   
    public class CustomView : ListView
    {
        public event EventHandler<EventArgs> LongPressEvent;

        public void RaiseLongPressEvent()
        {
            if (IsEnabled)
                LongPressEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
