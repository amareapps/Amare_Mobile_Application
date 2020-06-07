using Android.Database;
using Chatter.Classes;
using Chatter.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewProfile 
    {
        UserModel userModel = new UserModel();
        ApiConnector api = new ApiConnector();
        ObservableCollection<GalleryModel> galleryModel = new ObservableCollection<GalleryModel>();
        ObservableCollection<InstagramPhotosModel> instagramPhotos = new ObservableCollection<InstagramPhotosModel>();
        string userId;
        public ViewProfile(string id)
        {
            InitializeComponent();
            userId = id;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await loadUser();
        }
        async Task loadUser()
        {
            try
            {
                var user = await api.getSpeificUser(userId);
                var list = await api.otherUserImageList(userId);
                var igPhotos = await api.getIgPhotos(userId);
                BindingContext = user;
                foreach (GalleryModel model in list)
                {
                    galleryModel.Add(model);
                }
                galleryView.ItemsSource = galleryModel;
                //await DisplayAlert("Hayss", igPhotos, "Okay");
                if (igPhotos == null)
                {
                    instaFrame.IsVisible = false;
                    instaLayout.IsVisible = false;
                    return;
                }
                foreach (var modeler in igPhotos)
                {
                    instagramPhotos.Add(modeler);
                }
                BindableLayout.SetItemsSource(instagramAlbum, instagramPhotos);
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Error",ex.ToString(),"Okay");
            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            List<string> images = new List<string>();
            foreach (GalleryModel imageUrl in galleryModel)
            {
                images.Add(imageUrl.image);
            }
            await Navigation.PushModalAsync(new NavigationPage(new ImageViewer(images,"Photos")));
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            List<string> images = new List<string>();
            foreach (InstagramPhotosModel imageUrl in instagramPhotos)
            {
                images.Add(imageUrl.image_url);
            }
            await Navigation.PushModalAsync(new NavigationPage(new ImageViewer(images,"Instagram Photos")));
        }

        private void backButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void reportUser_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new ReportUserEntry()));
        }
    }
}