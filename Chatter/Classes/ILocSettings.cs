using System;
using System.Collections.Generic;
using System.Text;
[assembly: Xamarin.Forms.Dependency(typeof(Chatter.Classes.ILocSettings))]
namespace Chatter.Classes
{
    public interface ILocSettings
    {
        void OpenSettings();
    }
}
