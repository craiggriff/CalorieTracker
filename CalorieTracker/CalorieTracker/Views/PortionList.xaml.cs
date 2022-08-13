using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CalorieTracker.Models;
using System.Text.Json;
using System.Net.Http;
using System.Collections.ObjectModel;

namespace CalorieTracker
{
    public partial class PortionList : ContentPage
    {
        bool bSelected = false;
        HttpClient client;
        JsonSerializerOptions serializerOptions;
        bool bRedrawList = false;

        public class ListDataOC : ObservableCollection<ListData>
        {
            public ListDataOC() { }
        }

        public static ListDataOC dataSource = new ListDataOC();

        public PortionList()
        {
            InitializeComponent();
            client = new HttpClient();
            serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

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
           
            if (App.Database.SettingsRecord.Admin == true)
                total_bar.IsVisible = false;
            else
                total_bar.IsVisible = true;

            CreateList();
            bRedrawList = false;
            Device.BeginInvokeOnMainThread(StartSend);
        }

        private async void StartSend()
        {
            try
            {
                foreach (var p in App.Database.GetUnsentPortions())
                {
                    await SendPortionJson(p);

                    if (bRedrawList == true)
                    {
                        var item = dataSource.FirstOrDefault(i => i.RecID == p.RecID);
                        if (item != null)
                        {
                            item.text_colour = "#111166";
                        }
                        bRedrawList = false;
                    }
                }
            }
            catch (Exception e)
            {
                // Connection problem, will try again later
            }
        }

        public async Task SendPortionJson(PortionItem item)
        {
            byte[] image_binary = null;

            Uri uri = new Uri(string.Format(App.Database.SettingsRecord.ServerURL + "/ReceivePortionJson", string.Empty));

            try
            {
                PortionDTOSend send_record = new PortionDTOSend(item);

                if (App.Files.FileExists(item.RecID.ToString() + ".jpg"))
                {
                    image_binary = App.Files.LoadBinary(item.RecID.ToString() + ".jpg");
                }
                send_record.Photo = image_binary != null ? System.Convert.ToBase64String(image_binary) : "";

                string json = JsonSerializer.Serialize<PortionDTOSend>(send_record, serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;

                response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string receive_content = await response.Content.ReadAsStringAsync();
                    PortionDTO receive_record = JsonSerializer.Deserialize<PortionDTO>(receive_content, serializerOptions);

                    item.Sent = true;
                    item.DatabaseId = receive_record.Id;

                    App.Database.SavePortionRecord(item);
                    bRedrawList = true;
                }
            }
            catch (Exception ex)
            {
                return;
            }
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
            dataSource.Clear();

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
