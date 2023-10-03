using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CalorieTracker.ViewModel
{
    internal class PortionViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }

        public Command BackPressCommand { get; set; }
        public Command SavePortionCommand { get; }

        public PortionViewModel(INavigation navigation)
        {
            Navigation = navigation;
            SavePortionCommand = new Command(SaveCommand);

            BackPressCommand = new Command(BackCommand);
        }

        public void BackCommand()
        {
            App.Database.SavePortionRecord();

            Navigation.PopAsync();
        }

        public void SaveCommand()
        {
            App.Database.PortionRecord.Completed = true;

            App.Database.SavePortionRecord();

            Navigation.PopAsync();

            return;
        }


        public string Desc
        {
            get { return App.Database.PortionRecord.Product; }
            set
            {
                App.Database.PortionRecord.Product = value;

                OnPropertyChanged();
            }
        }

        public int Calories
        {
            get { return App.Database.PortionRecord.Calories; }
            set
            {
                App.Database.PortionRecord.Calories = value;

                OnPropertyChanged();
            }
        }

        public string Date
        {
            get { return App.Database.PortionRecord.Date; }
            set
            {
                App.Database.PortionRecord.Date = value;

                OnPropertyChanged();
            }
        }

        public string Time
        {
            get { return App.Database.PortionRecord.Time; }
            set
            {
                App.Database.PortionRecord.Time = value;

                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        


        //private 
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
            /*
            string error_text = "";

            if (App.Database.PortionRecord.Product == "")
                error_text = error_text + "You must enter a description\n";

            if (IsAllDigits(cal_entry.TextBinding) && (App.Database.PortionRecord.Calories < 1 || App.Database.PortionRecord.Calories > 2000))
                error_text = error_text + "Calories must be a whole number between 1 and 2000\n";

            if (error_text != "")
            {
                App.Database.PortionRecord.Completed = false;
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
                App.Database.PortionRecord.Completed = true;
                App.Database.SavePortionRecord();
                Navigation.PopAsync();
            }
            */
        }


    }
}
