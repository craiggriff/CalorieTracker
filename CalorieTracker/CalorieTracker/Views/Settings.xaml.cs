using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CalorieTracker.Models;

namespace CalorieTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
            BindingContext = App.Database.SettingsRecord as SettingsItem;
            if(App.Database.SettingsRecord.Admin==true)
                admin_options.IsVisible = true;
        }
        private void OnSaveClicked(object sender, EventArgs e)
        {
            App.Database.SettingsRecord.UserToken = App.Database.SettingsRecord.UserToken.ToUpper().Trim();
            App.Database.SettingsRecord.AdminPassword = App.Database.SettingsRecord.AdminPassword.ToUpper().Trim();
            if (App.Database.SettingsRecord.AdminPassword == "ADMIN")
                App.Database.SettingsRecord.Admin = true;
            else
                App.Database.SettingsRecord.Admin = false;
            App.Database.SaveSettings();
            Navigation.PopAsync();
        }
        private async void OnClearAllClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("", "Delete all data?", "   Yes   ", "   No   "))
            {
                App.Database.DeleteAllPortionRecords();
                App.Database.SettingsRecord.UserToken = "";
                App.Database.SettingsRecord.AdminPassword = "";
                App.Database.SettingsRecord.Admin = false;
                App.Database.SaveSettings();
                await Navigation.PopAsync();
            }
        }
        private async void OnAddTestClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("", "Create random test data?", "   Yes   ", "   No   "))
                App.Database.CreateTestData();
        }
    }
}