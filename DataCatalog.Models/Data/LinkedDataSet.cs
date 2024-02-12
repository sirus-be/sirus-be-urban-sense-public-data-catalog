using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataCatalog.Models
{
    public class LinkedDataSet : DataSet
    {
        [JsonPropertyName("@context")]
        [JsonProperty("@context")]
        public string Context { get; } = "https://context-location";

        [JsonPropertyName("@id")]
        [JsonProperty("@id")]
        public string Id => Identifier;

        public List<string> Roles { get; set; }
    }
}
