using LandonApi.Infrastructure;
using Newtonsoft.Json;

namespace LandonApi.Models
{
    public class HotelInfo : Resource, IEtaggable
    {
        public string Title { get; set; }
        public string TagLine { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public Address Location { get; set; }

        public string GetEtag()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return Md5Hash.ForString(serialized); //hash ktory bude pouzity v ETag headery
        }
    }
}
