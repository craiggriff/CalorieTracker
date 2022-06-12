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

namespace CalorieTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PortionView : ContentPage
    {
        public PortionView()
        {
            InitializeComponent();

            BindingContext = App.Database.PortionRecord as PortionTable;
            LoadImage();
        }
        private void OnSaveClicked(object sender, EventArgs e)
        {
            ValidateSaveAndClose();
        }
        bool IsAllDigits(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
        private void ValidateSaveAndClose()
        {
            string error_text = "";

            if (App.Database.PortionRecord.product == "")
                error_text = error_text + "You must enter a description\n";

            if (IsAllDigits(cal_entry.TextBinding) && (App.Database.PortionRecord.calories < 1 || App.Database.PortionRecord.calories>2000))
                error_text = error_text + "Calories must be a whole number between 1 and 2000\n";

            if (App.Files.FileExists(App.Database.PortionRecord.RecID.ToString() + ".jpg") == false)
                error_text = error_text + "No photograph taken\n";

            if (error_text != "")
            {
                App.Database.PortionRecord.b_completed = false;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var response = await DisplayAlert("Invalid",
                        "\n\n" + error_text + "\n\nSave as incomplete?\n", "   Yes   ", "   No   ");
                    if (response)
                    {
                        App.Database.SavePortionRecord();
                        await Navigation.PopAsync();
                    }
                });
            }
            else
            {
                App.Database.PortionRecord.b_completed = true;
                App.Database.SavePortionRecord();
                Navigation.PopAsync();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            ValidateSaveAndClose();
            return true;
        }
        
        private async void OnPhotoClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            App.Database.PortionRecord.b_sent = false;

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