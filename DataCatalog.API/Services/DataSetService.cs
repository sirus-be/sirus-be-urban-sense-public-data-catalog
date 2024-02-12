using Core.Collections;
using DataCatalog.API.Infrastructure.DataStores;
using DataCatalog.API.Models;
using DataCatalog.Models;
using DataCatalog.Models.Data;
using DataCatalog.Models.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataCatalog.API.Services
{
    public class DataSetService: IDataSetService
    {
        private readonly IDataSetStore _dataSetStore;

        public DataSetService(IDataSetStore dataSetStore)
        {
            _dataSetStore = dataSetStore ?? throw new ArgumentNullException(nameof(dataSetStore));
        }

        public async Task<IPaginatedList<LinkedDataSet>> GetAllDataSetsAsync(DataSetParameters dataSetParameters)
        {
            var items = await _dataSetStore.GetDataSetsAsync(dataSetParameters);
            if (items == null)
            {
                return null;
            }

            var itemsViewModel = items.Select(x => x.Data);

            return itemsViewModel;
        }

        public async Task<LinkedDataSet> GetDataSetAsync (string id)
        {
            var item = await _dataSetStore.GetDataSetAsync(id);
            if(item == null)
            {
                return null;
            }

            var itemViewModel = item.Data;

            return itemViewModel;
        }

        public async Task<LinkedDataSet> CreateDataSetAsync(LinkedDataSet dataSet)
        {
            var entity = new DataSetEntity();
            entity.EntityId = dataSet.Identifier;
            entity.Type = dataSet.Type;
            entity.Data = dataSet;

            foreach (var roleName in dataSet.Roles) //Mss verplaatsen nr aparte call?
            {
                var role = await _dataSetStore.GetRoleByNameAsync(roleName);
                if (role == null)
                {
                    throw new ValidationException($"Role {roleName} does not exists");
                }

                entity.Roles.Add(role);
            }

            var savedentity = await _dataSetStore.AddDataSetAsync(entity);
            var itemViewModel = savedentity.Data;

            return itemViewModel;
        }

        public async Task<LinkedDataSet> UpdateDataSetAsync(LinkedDataSet dataSet)
        {
            var entity = await _dataSetStore.GetDataSetAsync(dataSet.Id);
            if (entity == null)
            {
                throw new ValidationException($"Dataset with id {dataSet.Id} does not exists");
            }

            entity.Roles = new List<RoleEntity>();

            foreach (var roleName in dataSet.Roles) //Mss verplaatsen nr aparte call?
            {
                var role = await _dataSetStore.GetRoleByNameAsync(roleName);
                if (role == null)
                {
                    throw new ValidationException($"Role {roleName} does not exists");
                }

                entity.Roles.Add(role);
            }

            entity.EntityId = dataSet.Identifier;
            entity.Type = dataSet.Type;
            entity.Data = dataSet;

            var savedentity = await _dataSetStore.UpdateDataSetAsync(entity);
            
            var itemViewModel = savedentity.Data;

            return itemViewModel;
        }

        public async Task DeleteDataSetAsync(string id)
        {
            var entity = await _dataSetStore.GetDataSetAsync(id);
            if (entity != null)
            {
                await _dataSetStore.DeleteDataSetAsync(entity);
            }
        }

        public async Task<IPaginatedList<Role>> GetAllRolesAsync(RoleParameters roleParameters)
        {
            var items = await _dataSetStore.GetRolesAsync(roleParameters);
            var itemsViewModel = items.Select(x => x.ToViewModel());

            return itemsViewModel;
        }

        public async Task<IPaginatedList<Role>> GetAllRolesAsync(string dataSetName, RoleParameters roleParameters)
        {
            var dataSet = await _dataSetStore.GetDataSetAsync(dataSetName);
            if(dataSet == null)
            {
                throw new ValidationException("DataSet does not exists");
            }
            var items = await _dataSetStore.GetRolesAsync(dataSetName, roleParameters);
            var itemsViewModel = items.Select(x => x.ToViewModel());

            return itemsViewModel;
        }


        public async Task<Role> GetRoleByNameAsync(string name)
        {
            var item = await _dataSetStore.GetRoleByNameAsync(name);
            if (item == null)
            {
                return null;
            }

            return item.ToViewModel();
        }

        public async Task<Role> CreateRoleAsync(Role role)
        {
            var entity = new RoleEntity();
            entity.Name = role.Name;
            entity.Description = role.Description;

            var savedentity = await _dataSetStore.AddRoleAsync(entity);

            return savedentity.ToViewModel();
        }

        public async Task<Role> UpdateRoleAsync(UpdateRole role)
        {
            var entity = await _dataSetStore.GetRoleByNameAsync(role.PreviousName);
            if (entity == null)
            {
                throw new ValidationException($"Role {role.PreviousName} does not exists");
            }
            if(role.PreviousName != role.NewName)
            {
                var newEntity = await _dataSetStore.GetRoleByNameAsync(role.NewName);
                if(newEntity != null)
                {
                    throw new ValidationException($"Role with new name {role.NewName} already exists");
                }
            }
            entity.Name = role.NewName;
            entity.Description = role.Description;

            var savedentity = await _dataSetStore.UpdateRoleAsync(entity);

            return savedentity.ToViewModel();
        }
    }
}
