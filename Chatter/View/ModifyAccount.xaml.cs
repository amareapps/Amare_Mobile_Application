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
    public partial class ModifyAccount : ContentPage
    {
        public ModifyAccount(string title,string fieldtoUpdate,string fieldEntry)
        {
            InitializeComponent();
            this.Title = title;
            accountFieldLabel.Text = fieldtoUpdate;
            accountFieldEntry.Text = fieldEntry;
            buttonUpdate.Text = "Update " + fieldtoUpdate;
        }
    }
}