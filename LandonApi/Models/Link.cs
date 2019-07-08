using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Models
{
    public class Link
    {
        public const string GetMethod = "GET";
        public const string PostMethod = "POST";

        public static Link To(string routeName, object routeValues = null)
            => new Link
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = GetMethod,
                Relations = null
            };

        public static Link ToCollection(string routeName, object routeValues = null)
        {
            return new Link
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = GetMethod,
                Relations = new[] { "collection" }
            };
        }

        public static Link ToForm(
            string routeName,
            object routeValues = null,
            string method = PostMethod,
            params string[] relations)
            => new Link
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = method,
                Relations = relations
            };

        [JsonProperty(Order = -4)] //zabezpeci umistenie property vramci json objektu co najvyssie
        public string Href { get; set; }
        [JsonProperty(Order = -3, 
                        PropertyName = "rel", //nastavenie nazvu property v JSON objekte
                        NullValueHandling = NullValueHandling.Ignore)] //null hodnota bude ignorovana pri serializacii
        public string[] Relations { get; set; }
        [JsonProperty(Order = -2,
                        DefaultValueHandling = DefaultValueHandling.Ignore, //default hodnota bude ignorovana pri serializacii
                        NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(GetMethod)]
        public string Method { get; set; }

        //stores the route name/values before being rewritten by the LinkRewritingFilter
        [JsonIgnore] //ignorovane properties pri serializacii do JSON
        public string RouteName { get; set; }
        [JsonIgnore]
        public object RouteValues { get; set; }
    }
}
