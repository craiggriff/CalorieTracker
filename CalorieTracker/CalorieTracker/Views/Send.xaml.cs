using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CalorieTracker.Models;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Net.Http;

namespace CalorieTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Send : ContentPage
    {
        HttpClient client;
        JsonSerializerOptions serializerOptions;
        public Send()
        {
            InitializeComponent();
            client = new HttpClient();
            serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            try
            {
                foreach (var p in App.Database.GetUnsentPortions())
                {
                    //SendPortionXML(p);
                    SendPortionJson(p);
                }
            }catch(Exception e)
            {
                DisplayAlert("Error", e.ToString(), "OK");
            }
            Device.BeginInvokeOnMainThread(CompleteDownload);
            
        }

        void CompleteDownload()
        {
            Navigation.PopAsync();
        }
        public async void SendPortionJson(PortionItem item)
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
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }


        public void SendPortionXML(PortionItem item)
        {
            byte[] image_binary = null;

            StringBuilder localpostData = new StringBuilder(128000);

            if (App.Files.FileExists(item.RecID.ToString() + ".jpg"))
            {
                image_binary = App.Files.LoadBinary(item.RecID.ToString() + ".jpg");
            }
            image_binary = null;
            XDocument localTree = new XDocument(
            new XElement("Portion",
            new XElement("UserToken", item.UserToken),
            new XElement("RecordID", item.RecID),
            new XElement("DatabaseID", item.DatabaseId.ToString()),
            new XElement("Date", item.Date),
            new XElement("Time", item.Time),
            new XElement("Calories", item.Calories),
            new XElement("Description", item.Product),
            new XElement("Picture", image_binary!=null ? System.Convert.ToBase64String(image_binary):"")));

            localpostData.Append(localTree.ToString());

            PortionDTO ret = postXMLData(App.Database.SettingsRecord.ServerURL + "/ReceivePortionXML", localpostData.ToString());
            if(ret!=null)
            {
                item.Sent = true;
                item.DatabaseId = ret.Id;

                App.Database.SavePortionRecord(item);
            }
        }

        public PortionDTO postXMLData(string destinationUrl, string requestXml)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
            request.ContentType = "application/xml";
            request.ContentLength = bytes.Length;
            request.Method = "POST";

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.Created)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();

                return JsonSerializer.Deserialize<PortionDTO>(responseStr, serializerOptions);

            }
            return null;
        }

    }
}