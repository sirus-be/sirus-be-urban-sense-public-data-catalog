using DataCatalog.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCatalog.Client
{
    public static class DataCatalogServiceUrlBuilder
    {
        private const string RoleBaseUri = "api/v1/Role";
        private const string DataSetUri = "api/v1/DataSet";

        #region Roles

        public static string GetRolesUrl()
        {
            return RoleBaseUri;
        }

        public static string GetRolesUrl(RoleParameters parameters)
        {
            return BuildUrl(GetRolesUrl() + "?", parameters);
        }

        public static string GetRolesUrl(string dataSetName, RoleParameters parameters)
        {
            var url = $"{GetRolesUrl()}/DataSet/{dataSetName}?";
            return BuildUrl(url, parameters);
        }
        public static string GetRoleUrl(string roleName)
        {
            var sbUrl = new StringBuilder();
            return sbUrl.Append($"{GetRolesUrl()}/{roleName}").ToString();
        }

        private static string BuildUrl(string url, RoleParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                url += $"search={parameters.Search}&";
            }
            if (!string.IsNullOrWhiteSpace(parameters.Sorting))
            {
                url += $"sorting={parameters.Sorting}&";
            }
            url += $"pageIndex={parameters.PageIndex}&pageSize={parameters.PageSize}";
            return url;
        }

        #endregion

        #region DataSet

        public static string GetDataSetsUrl()
        {
            return DataSetUri;
        }

        public static string GetDataSetsUrl(DataSetParameters parameters)
        {
            return BuildUrl(GetDataSetsUrl() + "?", parameters);
        }
        public static string GetDataSetUrl(string datasetId)
        {
            var sbUrl = new StringBuilder();
            return sbUrl.Append($"{GetDataSetsUrl()}/{datasetId}").ToString();
        }

        private static string BuildUrl(string url, DataSetParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                url += $"search={parameters.Search}&";
            }
            if (!string.IsNullOrWhiteSpace(parameters.Sorting))
            {
                url += $"sorting={parameters.Sorting}&";
            }
            url += $"pageIndex={parameters.PageIndex}&pageSize={parameters.PageSize}";
            return url;
        }

        #endregion
    }
}
