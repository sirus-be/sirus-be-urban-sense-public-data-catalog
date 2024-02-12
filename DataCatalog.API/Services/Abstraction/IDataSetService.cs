using Core.Collections;
using DataCatalog.Models;
using DataCatalog.Models.Data;
using DataCatalog.Models.Parameters;
using System.Threading.Tasks;

namespace DataCatalog.API.Services
{
    public interface IDataSetService
    {
        Task<IPaginatedList<LinkedDataSet>> GetAllDataSetsAsync(DataSetParameters dataSetParameters);
        Task<LinkedDataSet> GetDataSetAsync(string id);
        Task<LinkedDataSet> CreateDataSetAsync(LinkedDataSet dataSet);
        Task<LinkedDataSet> UpdateDataSetAsync(LinkedDataSet dataSet);
        Task DeleteDataSetAsync(string id);

        Task<IPaginatedList<Role>> GetAllRolesAsync(RoleParameters roleParameters);
        Task<IPaginatedList<Role>> GetAllRolesAsync(string dataSetName, RoleParameters roleParameters);
        Task<Role> GetRoleByNameAsync(string name);
        Task<Role> CreateRoleAsync(Role role);
        Task<Role> UpdateRoleAsync(UpdateRole role);
    }
}