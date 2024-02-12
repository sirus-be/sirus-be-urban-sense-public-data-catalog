using Core.Collections;
using DataCatalog.API.Models;
using DataCatalog.Models.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace DataCatalog.API.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<IPaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> collection, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }

            var totalItems = await collection.CountAsync(cancellationToken);
            if (pageSize > 0)
            {
                var items = await collection.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);
                return new PaginatedList<T>(pageIndex, pageSize, totalItems, items);
            }
            else
            {
                var items = await collection.ToListAsync(cancellationToken);
                return new PaginatedList<T>(0, totalItems, totalItems, items);
            }
        }

        public static IQueryable<DataSetEntity> FilterOnRoles(this IQueryable<DataSetEntity> query, ClaimsPrincipal currentUser)
        {
            var userRoles = currentUser.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!userRoles.Contains(Roles.SuperAdmin))
            {
                query = query.Where(dataSet => dataSet.Roles.Any(dataSetRole => userRoles.Any(userRole => dataSetRole.Name == userRole)));
            }

            return query;
        }

        public static IQueryable<RoleEntity> FilterOnRoles(this IQueryable<RoleEntity> query, ClaimsPrincipal currentUser)
        {
            var userRoles = currentUser.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!userRoles.Contains(Roles.SuperAdmin))
            {
                query = query.Where(role => userRoles.Any(userRole => role.Name == userRole));
            }

            return query;
        }
    }
}
