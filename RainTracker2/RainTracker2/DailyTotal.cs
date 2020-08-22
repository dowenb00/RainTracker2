using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RainTracker2
{
    public class DailyTotal
    {
        [JsonProperty("locationkey")]
        public string LocationKey { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("inches")]
        public Double inches { get; set; }

    }
}
