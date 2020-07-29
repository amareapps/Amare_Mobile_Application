using Stripe;
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
    public partial class Payment : ContentPage
    {
        public Payment()
        {
            InitializeComponent();
        }
        string mycustomer;
        string getchargedID;
        string refundID;

        [Obsolete]
        private async void Button_Clicked(object sender, EventArgs e)
        {
            StripeConfiguration.SetApiKey("sk_test_51H7Lu7KJ3FKVwUnlPU8EUYuDPU0UMWNajFeHpVYzqwLkpKFRk9480iV54ZvAIPy4J0xYlKoN9IQaGMoyhhcaxOgl003Kz8FIdL");

            //This are the sample test data use MVVM bindings to send data to the ViewModel

            Stripe.CreditCardOptions stripcard = new Stripe.CreditCardOptions();
            stripcard.Number = cardNumberEntry.Text;
            stripcard.ExpYear = expiryDate.Date.Year;
            stripcard.ExpMonth = expiryDate.Date.Month;
            stripcard.Cvc = cvvEntry.Text;


            //Step 1 : Assign Card to Token Object and create Token

            Stripe.TokenCreateOptions token = new Stripe.TokenCreateOptions();
            token.Card = stripcard;
            Stripe.TokenService serviceToken = new Stripe.TokenService();
            Stripe.Token newToken = serviceToken.Create(token);

            // Step 2 : Assign Token to the Source

            var options = new SourceCreateOptions
            {
                Type = SourceType.Card,
                Currency = "usd",
                Token = newToken.Id
            };

            var service = new SourceService();
            Source source = service.Create(options);

            //Step 3 : Now generate the customer who is doing the payment

            Stripe.CustomerCreateOptions myCustomer = new Stripe.CustomerCreateOptions()
            {
                Name = nameEntry.Text,
                Email = emailEntry.Text,
                Description = "Amare Payment",
            };

            var customerService = new Stripe.CustomerService();
            Stripe.Customer stripeCustomer = customerService.Create(myCustomer);

            mycustomer = stripeCustomer.Id; // Not needed

            //Step 4 : Now Create Charge Options for the customer. 

            var chargeoptions = new Stripe.ChargeCreateOptions
            {
                Amount = 100,
                Currency = "USD",
                ReceiptEmail = emailEntry.Text,
                Customer = stripeCustomer.Id,
                Source = source.Id

            };

            //Step 5 : Perform the payment by  Charging the customer with the payment. 
            var service1 = new Stripe.ChargeService();
            Stripe.Charge charge = service1.Create(chargeoptions); // This will do the Payment

            getchargedID = charge.Id; // Not needed
            await DisplayAlert("Payment","Payment Success","Okay");
            await Navigation.PopAsync();
        }
    }
}