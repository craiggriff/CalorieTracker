using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;
using CalorieTracker.ViewModel;

namespace CalorieTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PortionView : ContentPage
    {
        public PortionView()
        {
            InitializeComponent();

            //BindingContext = App.Database.PortionRecord as PortionTable;
            BindingContext = new PortionViewModel(Navigation);
            LoadImage();
        }


        protected override bool OnBackButtonPressed()
        {
            var vm = (PortionViewModel)BindingContext;
            if (vm.BackPressCommand.CanExecute(null))  // You can add parameters if any
            {
                vm.BackPressCommand.Execute(null); // You can add parameters if any
            }

            return true;
        }
        private async void OnPhotoClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            App.Database.PortionRecord.Sent = false;

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Photos",
                Name = App.Database.PortionRecord.RecID.ToString() + ".jpg",
                PhotoSize = PhotoSize.Medium
            });

            if (file != null)
            {
                App.Files.SaveStream(file.GetStream(), App.Database.PortionRecord.RecID.ToString() + ".jpg");
                LoadImage();
            }
        }
        void LoadImage()
        {
            Picture.Source = ImageSource.FromFile(App.Files.AddPathToFilename(App.Database.PortionRecord.RecID.ToString() + ".jpg"));
            if (App.Files.FileExists(App.Database.PortionRecord.RecID.ToString() + ".jpg"))
                photo_button.IsVisible = false;
        }
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Delete Food Portion?", "", "   Yes   ", "   No   ");
            if (answer == true)
            {
                App.Database.DeleteCurrentPortionRecord();
                await Navigation.PopAsync();
            }
        }
    }
}