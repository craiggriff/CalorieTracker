using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CalorieTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
            BindingContext = App.Database.SettingsRecord as SettingsTable;
            if(App.Database.SettingsRecord.b_admin==true)
                admin_options.IsVisible = true;
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            App.Database.SettingsRecord.user_token = App.Database.SettingsRecord.user_token.ToUpper().Trim();
            App.Database.SettingsRecord.admin_password = App.Database.SettingsRecord.admin_password.ToUpper().Trim();
            if (App.Database.SettingsRecord.admin_password == "ADMIN")
                App.Database.SettingsRecord.b_admin = true;
            else
                App.Database.SettingsRecord.b_admin = false;
            App.Database.SaveSettings();
            Navigation.PopAsync();
        }
        private async void OnClearAllClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("", "Delete all data?", "   Yes   ", "   No   "))
            {
                App.Database.DeleteAllPortionRecords();
                App.Database.SettingsRecord.user_token = "";
                App.Database.SettingsRecord.admin_password = "";
                App.Database.SettingsRecord.b_admin = false;
                App.Database.SaveSettings();
                Navigation.PopAsync();
            }
        }
        private async void OnAddTestClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("", "Create random test data?", "   Yes   ", "   No   "))
                App.Database.CreateTestData();
        }
    }
}