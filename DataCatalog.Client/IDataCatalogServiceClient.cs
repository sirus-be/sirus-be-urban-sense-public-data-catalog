using Core.Collections;
using DataCatalog.Models;
using DataCatalog.Models.Data;
using DataCatalog.Models.Parameters;
using System.Threading.Tasks;

namespace DataCatalog.Client
{
    public interface IDataCatalogServiceClient
    {
        #region Role
        Task<PaginatedList<Role>> GetRolesAsync(RoleParameters parameters);
        Task<PaginatedList<Role>> GetDataSetRolesAsync(string dataSetName, RoleParameters parameters);
        Task<Role> GetRoleAsync(string name);
        Task<Role> PostRoleAsync(Role role);
        Task<UpdateRole> PutRoleAsync(UpdateRole role);
        #endregion

        #region DataSet
        Task<PaginatedList<LinkedDataSet>> GetDataSetsAsync(DataSetParameters parameters);
        Task<LinkedDataSet> GetDataSetAsync(string id);
        Task<LinkedDataSet> PostDataSetAsync(LinkedDataSet linkedDataSet);
        Task<LinkedDataSet> PutDataSetAsync(LinkedDataSet linkedDataSet);
        Task DeleteDataSetAsync(string dataSetId);
        #endregion
    }
}
