using Newtonsoft.Json;

namespace Xbim.Ifc
{
    public class XbimColourDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("red")]
        public double Red { get; set; }

        [JsonProperty("green")]
        public double Green { get; set; }

        [JsonProperty("blue")]
        public double Blue { get; set; }

        [JsonProperty("alpha")]
        public double Alpha { get; set; }

        public XbimColour ToXbimColour()
        {
            return new XbimColour(Name, Red, Green, Blue, Alpha);
        }
    }
}
