using System;
using System.Net;
using Newtonsoft.Json.Linq;
using TebexSE.Commands;

namespace TebexSE
{
    public class TebexApiClient : WebClient
    {

        //time in milliseconds
        private TebexSE plugin;
        private int timeout;
        public int Timeout
        {
            get
            {
                return timeout;
            }
            set
            {
                timeout = value;
            }
        }

        public void setPlugin(TebexSE plugin)
        {
            this.plugin = plugin;
        }

        public TebexApiClient(int timeout = 5000)
        {
            this.timeout = timeout;
        }

        public void DoGet(string endpoint, TebexCommandModule command)
        {
            this.Headers.Add("X-Buycraft-Secret", plugin.getSecret());
            String url = plugin.getBaseUrl() + endpoint;
            
            this.DownloadStringCompleted += (sender, e) =>
            {
                if (!e.Cancelled && e.Error == null)
                {
                    command.HandleResponse(JObject.Parse(e.Result));    
                }
                else
                {
                    command.HandleError(e.Error);
                }
                this.Dispose();
            };
            this.DownloadStringAsync(new Uri(url));
        }

        public void DoGet(string endpoint, Action<string> callback)
        {
            this.Headers.Add("X-Buycraft-Secret", plugin.getSecret());
            String url = plugin.getBaseUrl() + endpoint;

            this.DownloadStringCompleted += (sender, e) =>
            {
                if (!e.Cancelled && e.Error == null)
                {                    
                    callback(e.Result);
                }
                else
                {
                    TebexSE.log("error", "We are unable to process this API request.");
                    TebexSE.log("error", e.ToString());
                }
                this.Dispose();
            };
            this.DownloadStringAsync(new Uri(url));
        }
             
    }
}