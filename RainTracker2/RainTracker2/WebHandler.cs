using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
//using Plugin.Connectivity;
using System.Collections.ObjectModel;
using System.Web;
using Xamarin.Forms.GoogleMaps;

namespace RainTracker2
{
    public class WebHandler   //Sends package to AWS API Gateway, which forwards to Lambda service (see AWS Lambda folder)
    {
        HttpClient client;

        public WebHandler()
        {
            client = new HttpClient();
            //URI for AWS API Gateway
            client.BaseAddress = new Uri("https://xxxxxxxx.execute-api.us-east-2.amazonaws.com");
        }
        public async void AddLocation(double latitude, double longitude)
        {
            LambdaPackage lp = new LambdaPackage();
            lp.Function = "addlocation";
            lp.Latitude = latitude;
            lp.Longitude = longitude;
            string json = JsonConvert.SerializeObject(lp);
            string raw = await ExecuteDataRequest(json);
        }

        public async Task<List<Location>> GetLocationData()
        {
            LambdaPackage lp = new LambdaPackage();
            lp.Function = "getlocations";
            string json = JsonConvert.SerializeObject(lp);
            string raw = await ExecuteDataRequest(json);

            List<Location> locations = new List<Location>();
            locations = JsonConvert.DeserializeObject<List<Location>>(raw);

            lp.Function = "gettotals";
            json = JsonConvert.SerializeObject(lp);
            raw = await ExecuteDataRequest(json);

            List<DailyTotal> totals = new List<DailyTotal>();
            totals = JsonConvert.DeserializeObject<List<DailyTotal>>(raw);

            foreach (Location loc in locations)
            {
                foreach (DailyTotal total in totals)
                {
                    if (loc.LocationKey == total.LocationKey)
                    {
                        loc.Totals.Add(total);
                        loc.SortTotals();
                    }
                }
            }
            return locations;
        }

        public async Task<List<DailyTotal>> GetTotals()
        {
            LambdaPackage lp = new LambdaPackage();
            lp.Function = "gettotals";
            string json = JsonConvert.SerializeObject(lp);
            string raw = await ExecuteDataRequest(json);

            return JsonConvert.DeserializeObject<List<DailyTotal>>(raw);
        }

        public async Task<string> ExecuteDataRequest(string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await client.PostAsync("/stage1/api", content);
            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject(result).ToString();
        }
    }
}