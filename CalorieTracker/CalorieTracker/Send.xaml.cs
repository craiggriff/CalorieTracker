using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CalorieTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Send : ContentPage
    {
        StringBuilder postData = new StringBuilder(128000);

        List<PortionTable> portions = null;

        int total_portions = 0;
        int current_portion = 0;

        byte[] image_binary;

        string sendResponse = "";

        public Send()
        {
            InitializeComponent();

            portions = App.Database.GetUnsentPortions();

            total_portions = portions.Count();

            if (total_portions == 0)
                Device.BeginInvokeOnMainThread(CompleteDownload);
            else
                try
                {
                    SendNextPortion();
                }
                catch (Exception)
                {
                    DisplayAlert("error sending", current_portion.ToString(), "OK");
                    Navigation.PopAsync(false);
                }
        }

        public void SendNextPortion()
        {
            bool bImage = false;

            StringBuilder localpostData = new StringBuilder(128000);

            PortionTable item = portions[current_portion];

            if (App.Files.FileExists(item.RecID.ToString() + ".jpg"))
            {
                image_binary = App.Files.LoadBinary(item.RecID.ToString() + ".jpg");
                if(image_binary!=null)bImage = true;
            }

            XDocument localTree = new XDocument(
            new XElement("Portion",
            new XElement("UserToken", item.UserToken),
            new XElement("RecordID", item.RecID),
            new XElement("DatabaseID", item.DatabaseID),
            new XElement("Date", item.Date),
            new XElement("Time", item.Time),
            new XElement("Calories", item.Calories),
            new XElement("Description", item.Product),
            new XElement("Picture", bImage?System.Convert.ToBase64String(image_binary):"")));

            localpostData.Append(localTree.ToString());

            Uri thisuri = new Uri(App.Database.SettingsRecord.ServerURL + "/ReceivePortion");
            HttpHelper helper = new HttpHelper(thisuri, "POST",
            new KeyValuePair<string, string>("Portion", localpostData.ToString()));
            helper.ResponseComplete += new HttpResponseCompleteEventHandler(this.CommandComplete);
            helper.Execute();
        }

        private void CommandComplete(HttpResponseCompleteEventArgs e)
        {
            sendResponse = e.Response;

            if(sendResponse.Length>6)
            {
                if(sendResponse.Substring(0,6)=="Saved:")
                {
                    portions[current_portion].DatabaseID = sendResponse.Substring(6, sendResponse.Length - 6);
                    portions[current_portion].Sent = true;
                    App.Database.SavePortionRecord(portions[current_portion]);
                }
            }

            Device.BeginInvokeOnMainThread(CompleteDownload);
        }

        private void CompleteDownload()
        {
            current_portion++;
            if (current_portion < total_portions)
            {
                sending_progress.ProgressTo((1.0f / total_portions) * (float)current_portion, 250, Easing.Linear);
                try
                {
                    SendNextPortion();
                }
                catch (Exception)
                {
                    DisplayAlert("error sending", current_portion.ToString(), "OK");
                    Navigation.PopAsync(false);
                }
            }
            else
            {
                Navigation.PopAsync(false);
            }
        }
    }
}