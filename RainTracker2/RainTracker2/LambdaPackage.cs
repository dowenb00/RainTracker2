using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace RainTracker2
{
    public class LambdaPackage
    {
        [JsonProperty("function")]
        public string Function { get; set; }

        [JsonProperty("table")]
        public string Table { get; set; }

        [JsonProperty("param")]
        public string Param { get; set; }

        [JsonProperty("param_name")]
        public string ParamName { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("field_name")]
        public string FieldName { get; set; }

        [JsonProperty("guid")]
        public string GUID { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }

        [JsonProperty("ids")]
        public List<string> Ids { get; set; }

        public LambdaPackage() { }
        public LambdaPackage(string[] ids)
        {
            Ids = ids.ToList();
        }
    }
}
