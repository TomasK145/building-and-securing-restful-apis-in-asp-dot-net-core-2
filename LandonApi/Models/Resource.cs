using Newtonsoft.Json;

namespace LandonApi.Models
{
    public abstract class Resource
    {
        [JsonProperty(Order = -2)] //zabezpeci ze property bude na vrchu serializovanych responses
        public string Href { get; set; }
    }
}
