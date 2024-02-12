using Core.Collections;
using DataCatalog.API.Extensions;
using DataCatalog.API.Infrastructure.Database;
using DataCatalog.API.Models;
using DataCatalog.Models.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Security.Authentication;

namespace DataCatalog.API.Infrastructure.DataStores
{
    public class DataSetStore : IDataSetStore
    {
        private readonly ClaimsPrincipal _currentUser;
        private readonly string _userName;
        private readonly DataCatalogDbContext _dbContext;
        private readonly IAuthorizationService _authorizationService;

        public DataSetStore(DataCatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _currentUser = httpContextAccessor.HttpContext.User;
            _userName = httpContextAccessor.HttpContext.User.Claims
                        .Where(c => c.Type.Equals("preferred_username"))
                        .Select(c => c.Value)
                        .FirstOrDefault() ?? string.Empty;
        }

        public async Task<IPaginatedList<DataSetEntity>> GetDataSetsAsync(DataSetParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.DataSets.Where(i => !i.IsDeleted).FilterOnRoles(_currentUser);

            if (parameters != null)
            {
                query = FilterQuery(query, parameters);
            }

            return await query.ToPaginatedListAsync(parameters.PageIndex, parameters.PageSize, cancellationToken);
        }

        public async Task<DataSetEntity> GetDataSetAsync(string id, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.DataSets.Include(f => f.Roles).AsQueryable();

            var dataSet = await query.FirstOrDefaultAsync(i => i.EntityId.ToLower() == id.ToLower() && !i.IsDeleted, cancellationToken);

            return await AuthorizeRoles(dataSet);
        }

        public async Task<DataSetEntity> AddDataSetAsync(DataSetEntity dataSetEntity, CancellationToken cancellationToken = default)
        {
            dataSetEntity.CreatedOn = DateTime.Now;
            dataSetEntity.CreatedBy = _userName;
            dataSetEntity.LastModifiedBy = null;
            dataSetEntity.LastModifiedOn = null;

            var entityEntry = _dbContext.DataSets.Add(dataSetEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entityEntry.Entity;
        }

        public async Task<DataSetEntity> UpdateDataSetAsync(DataSetEntity dataSetEntity, CancellationToken cancellationToken = default)
        {
            var entityEntry = GetChangeTrackedEntity(dataSetEntity);
            entityEntry.Entity.LastModifiedBy = _userName;
            entityEntry.Entity.LastModifiedOn = DateTime.UtcNow;

            entityEntry.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entityEntry.Entity;
        }

        public async Task DeleteDataSetAsync(DataSetEntity dataSetEntity, CancellationToken cancellationToken = default)
        {
            var entityEntry = GetChangeTrackedEntity(dataSetEntity);
            entityEntry.Entity.IsDeleted = true;
            entityEntry.Entity.LastModifiedBy = _userName;
            entityEntry.Entity.LastModifiedOn = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private IQueryable<DataSetEntity> FilterQuery(IQueryable<DataSetEntity> query, DataSetParameters parameters)
        {
            if (parameters.Search != null)
            {
                query = query.Where(i => 
                i.Data.Description.ToLower().Contains(parameters.Search.ToLower()) ||
                i.Data.Title.ToLower().Contains(parameters.Search.ToLower()) ||
                i.Data.Publisher.Name.ToLower().Contains(parameters.Search.ToLower()));
            }

            if (!string.IsNullOrEmpty(parameters.Sorting))
            {
                switch (parameters.Sorting)
                {
                    case DataSetOrder.DescriptionAscending:
                        query = query.OrderBy(x => x.Data.Description);
                        break;
                    case DataSetOrder.DescriptionDescending:
                        query = query.OrderByDescending(x => x.Data.Description);
                        break;
                    case DataSetOrder.TitleAscending:
                        query = query.OrderBy(x => x.Data.Title);
                        break;
                    case DataSetOrder.TitleDescending:
                        query = query.OrderByDescending(x => x.Data.Title);
                        break;
                    case DataSetOrder.IdentifierAscending:
                        query = query.OrderBy(x => x.Data.Identifier);
                        break;
                    case DataSetOrder.IdentifierDescending:
                        query = query.OrderByDescending(x => x.Data.Identifier);
                        break;
                }
            }

            return query;
        }

        public async Task<IPaginatedList<RoleEntity>> GetRolesAsync(RoleParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Roles.Where(i => !i.IsDeleted).FilterOnRoles(_currentUser);

            if (parameters != null)
            {
                query = FilterQuery(query, parameters);
            }

            return await query.ToPaginatedListAsync(parameters.PageIndex, parameters.PageSize, cancellationToken);
        }

        public async Task<IPaginatedList<RoleEntity>> GetRolesAsync(string dataSetName, RoleParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Roles.Include(x => x.DataSets).Where(x => x.DataSets.Any(y => y.EntityId == dataSetName) && !x.IsDeleted);

            if(parameters != null)
            {
                query = FilterQuery(query, parameters);
            }
            return await query.ToPaginatedListAsync(parameters.PageIndex, parameters.PageSize, cancellationToken);
        }


        public async Task<RoleEntity> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            await AuthorizeRoles(new List<string> { name });

            var query = _dbContext.Roles.AsQueryable();

            return await query.FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower() && !i.IsDeleted, cancellationToken);
        }

        public async Task<RoleEntity> AddRoleAsync(RoleEntity role, CancellationToken cancellationToken = default)
        {
            role.CreatedOn = DateTime.Now;
            role.CreatedBy = _userName;
            role.LastModifiedBy = null;
            role.LastModifiedOn = null;

            var entityEntry = _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entityEntry.Entity;
        }

        public async Task<RoleEntity> UpdateRoleAsync(RoleEntity role, CancellationToken cancellationToken = default)
        {
            var entityEntry = GetChangeTrackedEntity(role);
            entityEntry.Entity.LastModifiedBy = _userName;
            entityEntry.Entity.LastModifiedOn = DateTime.UtcNow;

            entityEntry.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entityEntry.Entity;
        }

        private IQueryable<RoleEntity> FilterQuery(IQueryable<RoleEntity> query, RoleParameters parameters)
        {
            if (parameters.Search != null)
            {
                query = query.Where(i => i.Name.ToLower().Contains(parameters.Search.ToLower()) || i.Description.ToLower().Contains(parameters.Search.ToLower()));
            }

            if (!string.IsNullOrEmpty(parameters.Sorting))
            {
                query = query.OrderBy(parameters.Sorting);
            }

            return query;
        }

        private async Task<DataSetEntity> AuthorizeRoles(DataSetEntity DataSetEntity)
        {
            if (DataSetEntity != null)
            {
                await AuthorizeRoles(DataSetEntity.Roles.Select(r => r.Name));
            }

            return DataSetEntity;
        }

        private async Task AuthorizeRoles(IEnumerable<string> roles)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(_currentUser, roles, "ValidRoleRolesPolicy");
            if (!authorizationResult.Succeeded)
            {
                throw new AuthenticationException($"You don't have the right permissions.");
            }
        }

        private EntityEntry<TEntity> GetChangeTrackedEntity<TEntity>(TEntity item)
                where TEntity : class
        {
            var entityEntry = _dbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(i => i.Entity == item);
            if (entityEntry == null)
            {
                entityEntry = _dbContext.Set<TEntity>().Attach(item);
            }
            return entityEntry;
        }
    }
}
