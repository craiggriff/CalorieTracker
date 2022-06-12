using System;
using System.Collections.Generic;

using System.IO;

using System.Net;

namespace CalorieTracker
{
    public class HttpHelper
    {
        private HttpWebRequest Request { get; set; }
        public Dictionary<string, string> PostValues { get; private set; }

        public event HttpResponseCompleteEventHandler ResponseComplete;
        private void OnResponseComplete(HttpResponseCompleteEventArgs e)
        {
            if (this.ResponseComplete != null)
            {
                this.ResponseComplete(e);
            }
        }

        public HttpHelper(Uri requestUri, string method, params KeyValuePair<string, string>[] postValues)
        {
            this.Request = (HttpWebRequest)WebRequest.Create(requestUri);

            this.Request.UseDefaultCredentials = false;

            this.Request.ContentType = "application/x-www-form-urlencoded";
            this.Request.Method = method;

            //this.Request.
            //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            //this.Request.t
            this.PostValues = new Dictionary<string, string>();
            if (postValues != null && postValues.Length > 0)
            {
                foreach (var item in postValues)
                {
                    this.PostValues.Add(item.Key, item.Value);
                }
            }
        }

        public void Execute()
        {
            this.Request.BeginGetRequestStream(new AsyncCallback(HttpHelper.BeginRequest), this);
        }

        private static void BeginRequest(IAsyncResult ar)
        {
            HttpHelper helper = ar.AsyncState as HttpHelper;

            try
            {
                if (helper != null)
                {
                    if (helper.PostValues.Count > 0)
                    {
                        using (StreamWriter writer = new StreamWriter(helper.Request.EndGetRequestStream(ar)))
                        {
                            foreach (var item in helper.PostValues)
                            {
                                writer.Write("{0}={1}&", item.Key, System.Net.WebUtility.UrlEncode(item.Value));
                            }
                        }
                    }
                    helper.Request.BeginGetResponse(new AsyncCallback(HttpHelper.BeginResponse), helper);
                }
            }
            catch (Exception e)
            {
                helper.OnResponseComplete(new HttpResponseCompleteEventArgs("nointernet"));
            }
        }

        private static void BeginResponse(IAsyncResult ar)
        {
            HttpHelper helper = ar.AsyncState as HttpHelper;
            if (helper != null)
            {
                HttpWebResponse response;// = (HttpWebResponse)helper.Request.EndGetResponse(ar);

                try
                {
                    response = (HttpWebResponse)helper.Request.EndGetResponse(ar);
                }
                catch (WebException ex)
                {
                    response = ex.Response as HttpWebResponse;
                }

                if (response != null)
                {
                    Stream stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            helper.OnResponseComplete(new HttpResponseCompleteEventArgs(reader.ReadToEnd()));
                        }
                    }
                }
            }
        }
    }

    public delegate void HttpResponseCompleteEventHandler(HttpResponseCompleteEventArgs e);
    public class HttpResponseCompleteEventArgs : EventArgs
    {
        public string Response { get; set; }

        public HttpResponseCompleteEventArgs(string response)
        {
            this.Response = response;
        }
    }
}
