using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CalorieTracker.Models;

namespace CalorieTracker
{
    public partial class PortionList : ContentPage
    {
        public class ListData
        {
            public int RecID { get; set; }
            public string time { get; set; }
            public string date { get; set; }
            public string product { get; set; }
            public string calories { get; set; }
            public string back_colour { get; set; }
            public string text_colour { get; set; }
            public string user_token { get; set; }

            public ListData(int uID, string time, string date, string product, string calories, string user_token, bool b_completed, bool b_sent)
            {
                this.RecID = uID;
                this.time = time;
                this.date = date.Substring(0, 6) + date.Substring(8, 2);
                this.product = product;
                this.calories = calories;

                this.back_colour = "#e8c658";
                if(b_sent == true)
                    this.text_colour = "#111166";
                else
                    if(b_completed == true)
                        this.text_colour = "#116611";
                    else
                        this.text_colour = "#661111";

                this.user_token = user_token;

                if(App.Database.SettingsRecord.Admin && user_token.Length>2)
                {
                    // Generate a hex color based on User Token so it is eaier to see in the list
                    this.back_colour = "#" + Convert.ToString(((user_token[0] - 64) * 5) + 126, 16) 
                        + Convert.ToString(((user_token[2] - 64) * 5) + 126, 16) 
                        + Convert.ToString(((user_token[1] - 64) * 5) + 126, 16);
                }
            }
        }

        bool bSelected = false;
        
        public PortionList()
        {
            InitializeComponent();

            datepicker1.Date = DateTime.Today;
            datepicker2.Date = DateTime.Today;

            CreateList();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Title = "Calorie Tracker       User : " + (App.Database.SettingsRecord.Admin ? "Administrator" : App.Database.SettingsRecord.UserToken);
            bSelected = false;
            if (App.Database.SettingsRecord.Admin)
            {
                report_option.IconImageSource = ImageSource.FromFile("report.png");
                user_label.IsVisible = true;
            }
            else
            {
                report_option.IconImageSource = "";
                user_label.IsVisible = false;
            }
            int num_unsent = App.Database.GetUnsentPortions().Count;
            if(num_unsent>0)
            {
                send_unsent_button.Text = "Send (" + num_unsent.ToString() + ")";
                send_unsent_button.IsEnabled = true;
            }
            else
            {
                send_unsent_button.Text = "Send";
                send_unsent_button.IsEnabled = false;
            }

            if (App.Database.SettingsRecord.Admin == true)
                total_bar.IsVisible = false;
            else
                total_bar.IsVisible = true;

            CreateList();
        }
        private void OnBack(object sender, EventArgs e)
        {
            datepicker1.Date = datepicker1.Date.AddDays(-1);
            datepicker2.Date = datepicker1.Date;
        }
        private void OnNext(object sender, EventArgs e)
        {
            datepicker1.Date = datepicker1.Date.AddDays(1);
            datepicker2.Date = datepicker1.Date;
        }
        private void CreateList()
        {
            List<ListData> dataSource = new List<ListData>();

            List<PortionItem> query = App.Database.GetPortionsByDateRange(datepicker1.Date.ToShortDateString(), datepicker2.Date.ToShortDateString());

            if(App.Database.SettingsRecord.Admin)
                query = query.OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day).ThenBy(x => x.UserToken).ThenBy(x => x.Time).ToList();
            else
                query = query.OrderBy(x => x.Time).ToList();

            float total_cals = 0.0f;

            foreach (var item in query)
            {
                dataSource.Add(new ListData(item.RecID, item.Time, item.Date, item.Product, item.Calories.ToString(), App.Database.SettingsRecord.Admin?item.UserToken:"", item.Completed, item.Sent));
                if(item.Completed==true)
                    total_cals = total_cals + item.Calories;
            }
            total_calories.Text = total_cals.ToString();

            if (total_cals >= 2100)
                total_bar.BackgroundColor = Color.Pink;
            else
                total_bar.BackgroundColor = Color.AliceBlue;

            listView.ItemsSource = dataSource;
        }
        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (bSelected == false)
            {
                bSelected = true;
                App.Database.PortionRecord = App.Database.GetPortionByID((e.Item as ListData).RecID);
                if (App.Database.PortionRecord != null)
                Navigation.PushAsync(new PortionView(), false);
            }
        }
        private void OnSyncClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Send(), false);
        }
        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            if (datepicker1.Date > datepicker2.Date)
                datepicker2.Date = datepicker1.Date;
            CreateList();
        }
        private void OnToday(object sender, EventArgs e)
        {
            datepicker1.Date = DateTime.Today;
            datepicker2.Date = DateTime.Today;
            CreateList();
        }
        private void OnAddClicked(object sender, EventArgs e)
        {
            if (App.Database.SettingsRecord.UserToken == "")
            {
                DisplayAlert("", "Please enter a user token", "OK");
                Navigation.PushAsync(new Settings(), false);
            }
            else
            {
                App.Database.CreatePortionRecord();
                Navigation.PushAsync(new PortionView(), false);
            }
        }
        private void OnSettingsClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Settings(), false);
        }
        private void OnDateSelected2(object sender, DateChangedEventArgs e)
        {
            if (datepicker2.Date < datepicker1.Date)
                datepicker1.Date = datepicker2.Date;
            CreateList();
        }
        private void OnReportClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Report(), false);
        }
    }
}
