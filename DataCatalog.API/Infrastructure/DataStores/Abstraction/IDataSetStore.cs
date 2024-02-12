using Core.Collections;
using DataCatalog.API.Models;
using DataCatalog.Models.Parameters;
using System.Threading;
using System.Threading.Tasks;

namespace DataCatalog.API.Infrastructure.DataStores
{
    public interface IDataSetStore
    {
        Task<IPaginatedList<DataSetEntity>> GetDataSetsAsync(DataSetParameters dataSetParameters, CancellationToken cancellationToken = default);
        Task<DataSetEntity> GetDataSetAsync(string id, CancellationToken cancellationToken = default);
        Task<DataSetEntity> AddDataSetAsync(DataSetEntity dataSetEntity, CancellationToken cancellationToken = default);
        Task<DataSetEntity> UpdateDataSetAsync(DataSetEntity dataSetEntity, CancellationToken cancellationToken = default);
        Task DeleteDataSetAsync(DataSetEntity dataSetEntity, CancellationToken cancellationToken = default);

        Task<IPaginatedList<RoleEntity>> GetRolesAsync(RoleParameters roleParameters, CancellationToken cancellationToken = default);
        Task<IPaginatedList<RoleEntity>> GetRolesAsync(string dataSetName, RoleParameters roleParameters, CancellationToken cancellationToken = default);
        Task<RoleEntity> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<RoleEntity> AddRoleAsync(RoleEntity role, CancellationToken cancellationToken = default);
        Task<RoleEntity> UpdateRoleAsync(RoleEntity role, CancellationToken cancellationToken = default);
    }
}