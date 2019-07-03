using Newtonsoft.Json;

namespace LandonApi.Models
{
    public abstract class Resource : Link
    {
        //[JsonProperty(Order = -2)] //zabezpeci ze property bude na vrchu serializovanych responses
        //public string Href { get; set; }
        [JsonIgnore]
        public Link Self { get; set; }
    }
}
