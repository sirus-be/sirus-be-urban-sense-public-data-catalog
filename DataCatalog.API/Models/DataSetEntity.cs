using Core.Models;
using DataCatalog.Models;
using System.Collections.Generic;

namespace DataCatalog.API.Models
{
    public class DataSetEntity : AuditableEntity
    {
        public string EntityId { get; set; }
        public string Type { get; set; }
        public LinkedDataSet Data { get; set; }
        public string Context { get; set; }
        public List<RoleEntity> Roles { get; set; } = new List<RoleEntity>();

    }
}
