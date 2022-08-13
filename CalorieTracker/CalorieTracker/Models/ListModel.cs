using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CalorieTracker.Models
{
    public class ListData : INotifyPropertyChanged
    {
        public int RecID { get; set; }
        public string time { get; set; }
        public string date { get; set; }
        public string product { get; set; }
        public string calories { get; set; }
        public string back_colour { get; set; }
        public string _text_colour { get; set; }
        public string text_colour { get { return _text_colour; } set { _text_colour = value; NotifyPropertyChanged("text_colour"); } }

        public string user_token { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ListData(int uID, string time, string date, string product, string calories, string user_token, bool b_completed, bool b_sent)
        {
            this.RecID = uID;
            this.time = time;
            this.date = date.Substring(0, 6) + date.Substring(8, 2);
            this.product = product;
            this.calories = calories;

            this.back_colour = "#e8c658";
            if (b_sent == true)
                this.text_colour = "#111166";
            else
                if (b_completed == true)
                this.text_colour = "#116611";
            else
                this.text_colour = "#661111";

            this.user_token = user_token;

            if (App.Database.SettingsRecord.Admin && user_token.Length > 2)
            {
                // Generate a hex color based on User Token so it is eaier to see in the list
                this.back_colour = "#" + Convert.ToString(((user_token[0] - 64) * 5) + 126, 16)
                    + Convert.ToString(((user_token[2] - 64) * 5) + 126, 16)
                    + Convert.ToString(((user_token[1] - 64) * 5) + 126, 16);
            }
        }
    }
}
