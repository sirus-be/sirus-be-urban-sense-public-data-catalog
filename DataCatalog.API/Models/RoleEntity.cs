using Core.Models;
using DataCatalog.Models;
using System.Collections.Generic;

namespace DataCatalog.API.Models
{
    public class RoleEntity : AuditableEntity
    {
        public string Name { get; set; }
        public List<DataSetEntity> DataSets { get; set; } = new List<DataSetEntity>();

        public Role ToViewModel()
        {
            return new Role
            {
                Name = Name,
                Description = Description
            };
        }
    }
}
