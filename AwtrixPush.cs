using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace GlobalMute
{
    internal class AwtrixPush(string host, int indicator, string muteColor)
    {
        private readonly string host = host;
        private readonly int indicator = indicator;
        private readonly string muteColor = muteColor;

        public void PostMuteStateToDisplay(bool status)
        {
            var url = host + "/api/indicator" + indicator;
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };

            string color = "0";
            if (status)
            {
                color = muteColor;
            }

            var content = new StringContent("{\"color\":\"" + color + "\"}", Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;
            if (result == null || result.StatusCode != HttpStatusCode.OK)
            {
                Log.Error("Error pushing to AWTRIX");
                return;
            }
            Log.Information("AWTRIX Response: {Result}", result.Content.ReadAsStringAsync().Result);
            if (result.Content.ReadAsStringAsync().Result != "OK")
            {
                Log.Error("Invalid AWTRIX response");
            }
        }
    }
}
