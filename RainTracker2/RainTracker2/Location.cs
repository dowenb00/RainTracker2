using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace RainTracker2
{
    public class Location
    {
        [JsonProperty("locationkey")]
        public string LocationKey { get; set; }
        [JsonProperty("locationname")]
        public string LocationName { get; set; }
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        public List<DailyTotal> Totals { get; set; }
        
        public Location()
        {
            Totals = new List<DailyTotal>();
        }

        public void SortTotals()
        {
            Totals = Totals.OrderBy(o => o.Date).ToList();
        }
        public double GetSpanTotal(DateTime start, DateTime finish)
        {
            double total = 0;
            foreach (DailyTotal t in Totals)
            {
                if (t.Date >= start && t.Date <= finish)
                {
                    total += t.inches;
                }
            }
            return total;
        }
    }
}
