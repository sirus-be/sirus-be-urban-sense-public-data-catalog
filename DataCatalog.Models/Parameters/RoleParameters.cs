using System.Text.Json.Serialization;

namespace DataCatalog.Models.Parameters
{
    public class RoleParameters:Parameters
    {
        [JsonPropertyName("sorting")]
        public string Sorting { get; set; } = "Name asc";
    }
}
