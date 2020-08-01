using Google.Protobuf.WellKnownTypes;
using Java.Sql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Chatter.ViewModel
{
    public class CreditCardPageViewModel : INotifyPropertyChanged
    {
        public string carder,dater,cvver;
        public string CardNumber { get
            {
                return carder;
            }
            set {
                carder = value;
                onPropertyChanged("CardNumber");
            } 
        }
        public string CardCvv { 
            get {
                return cvver;
            } 
            set {
                cvver = value;
                onPropertyChanged("CardCvv");
            } 
        }
        public string CardExpirationDate {
            get
            {
                return dater;
            }
            set
            {
                dater = value;
                onPropertyChanged("CardExpirationDate");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void onPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
