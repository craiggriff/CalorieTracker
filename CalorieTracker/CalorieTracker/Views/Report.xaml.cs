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
    public partial class Report : ContentPage
    {
        public class ListData
        {
            public string user_token { get; set; }
            public string back_colour { get; set; }
            public string entries_last7days { get; set; }
            public string entries_7daysbefore { get; set; }
            public string entries_difference { get; set; }
            public string average_last7days { get; set; }
            public string current_day { get; set; }
            public ListData(string user_token, string entries_last7days, string entries_7daysbefore, string average_last7days, string current_day)
            {
                this.entries_last7days = entries_last7days;
                this.entries_7daysbefore = entries_7daysbefore;
                this.entries_difference = (int.Parse(entries_last7days)-int.Parse(entries_7daysbefore)).ToString();
                this.average_last7days = average_last7days;
                this.current_day = current_day;

                this.user_token = user_token;
                this.back_colour = "#" + Convert.ToString(((user_token[0] - 64) * 5) + 126, 16)
                        + Convert.ToString(((user_token[2] - 64) * 5) + 126, 16)
                        + Convert.ToString(((user_token[1] - 64) * 5) + 126, 16);
            }
        }
        public Report()
        {
            InitializeComponent();
            CreateReport();
        }
        private void CreateReport()
        {
            List<ListData> dataSource = new List<ListData>();

            List<PortionItem> query_last7days = App.Database.GetPortionsByDateRange(DateTime.Today.AddDays(-7).ToShortDateString(), DateTime.Today.ToShortDateString());
            List<PortionItem> query_7daysbefore = App.Database.GetPortionsByDateRange(DateTime.Today.AddDays(-14).ToShortDateString(), DateTime.Today.AddDays(-7).ToShortDateString());

            var user_tokens = query_last7days.Select(x => x.UserToken).Distinct();

            foreach (var user_token in user_tokens)
            {
                float last7days_daily_average = 0.0f;

                foreach(var portion in query_last7days.Where(x => x.UserToken == user_token))
                {
                    last7days_daily_average = last7days_daily_average + portion.Calories;
                }

                last7days_daily_average = last7days_daily_average / 7;

                dataSource.Add(new ListData(user_token, 
                    query_last7days.Where(x => x.UserToken == user_token).Count().ToString(), 
                    query_7daysbefore.Where(x => x.UserToken == user_token).Count().ToString(), 
                    ((int)last7days_daily_average).ToString(), 
                    DateTime.Today.ToString("dddd")));
            }
            listView.ItemsSource = dataSource;
        }
    }
}