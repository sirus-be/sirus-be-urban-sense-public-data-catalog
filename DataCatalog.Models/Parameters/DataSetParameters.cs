using System.Text.Json.Serialization;

namespace DataCatalog.Models.Parameters
{
    public class DataSetParameters:Parameters
    {
        [JsonPropertyName("sorting")]
        public string Sorting { get; set; } = "EntityId asc";
    }
}
