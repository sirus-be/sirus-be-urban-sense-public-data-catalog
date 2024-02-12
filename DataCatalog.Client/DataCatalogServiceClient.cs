using Core.Authentication;
using Core.Collections;
using DataCatalog.Models;
using DataCatalog.Models.Data;
using DataCatalog.Models.Parameters;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataCatalog.Client
{
    public class DataCatalogServiceClient : IDataCatalogServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly TokenProvider _tokenProvider;

        public DataCatalogServiceClient(HttpClient httpClient, TokenProvider tokenProvider)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _tokenProvider = tokenProvider;
        }

        #region Roles

        public async Task<PaginatedList<Role>> GetRolesAsync(RoleParameters parameters)
        {
            return await GetAllAsync<PaginatedList<Role>>(DataCatalogServiceUrlBuilder.GetRolesUrl(parameters));
        }
        public async Task<PaginatedList<Role>> GetDataSetRolesAsync(string dataSetName, RoleParameters parameters)
        {
            return await GetAllAsync<PaginatedList<Role>>(DataCatalogServiceUrlBuilder.GetRolesUrl(dataSetName ,parameters));
        }

        public async Task<Role> GetRoleAsync(string name)
        {
            return await GetAsync<Role>(DataCatalogServiceUrlBuilder.GetRoleUrl(name));
        }

        public async Task<Role> PostRoleAsync(Role role)
        {
            return await PostAsync<Role>(DataCatalogServiceUrlBuilder.GetRolesUrl(), role);
        }

        public async Task<UpdateRole> PutRoleAsync(UpdateRole role)
        {
            return await PutAsync<UpdateRole>(DataCatalogServiceUrlBuilder.GetRolesUrl(), role);
        }

        #endregion

        #region DataSet
        public async Task<PaginatedList<LinkedDataSet>> GetDataSetsAsync(DataSetParameters parameters)
        {
            return await GetAllAsync<PaginatedList<LinkedDataSet>>(DataCatalogServiceUrlBuilder.GetDataSetsUrl(parameters));
        }

        public async Task<LinkedDataSet> GetDataSetAsync(string id)
        {
            return await GetAsync<LinkedDataSet>(DataCatalogServiceUrlBuilder.GetDataSetUrl(id));
        }

        public async Task<LinkedDataSet> PostDataSetAsync(LinkedDataSet linkedDataSet)
        {
            return await PostAsync<LinkedDataSet>(DataCatalogServiceUrlBuilder.GetDataSetsUrl(), linkedDataSet);
        }

        public async Task<LinkedDataSet> PutDataSetAsync(LinkedDataSet linkedDataSet)
        {
            return await PutAsync<LinkedDataSet>(DataCatalogServiceUrlBuilder.GetDataSetsUrl(), linkedDataSet);
        }

        public async Task DeleteDataSetAsync(string dataSetId)
        {
            await DeleteAsync<DataSet>(DataCatalogServiceUrlBuilder.GetDataSetUrl(dataSetId));
        }
        #endregion

        #region Private methods
        private async Task<TResult> PostAsync<TResult>(string uri, TResult entity) where TResult : class
        {
            SetAuthorizationHeader(_tokenProvider.GetAccessToken());

            var content = JsonContent.Create(entity);
            var response = await _httpClient.PostAsync(uri, content);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var error = await response.Content.ReadAsAsync<HttpError>();
                    throw new Exception(error.Message);
                }
                throw new Exception(string.Empty);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResult>(responseContent);
        }

        private async Task<TResult> PutAsync<TResult>(string uri, TResult entity) where TResult : class
        {
            SetAuthorizationHeader(_tokenProvider.GetAccessToken());

            var content = JsonContent.Create(entity);
            var response = await _httpClient.PutAsync(uri, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new AuthenticationException(((int)response.StatusCode).ToString());
            }
            return JsonSerializer.Deserialize<TResult>(responseContent);
        }

        private async Task<TResult> GetAllAsync<TResult>(string uri)
            where TResult : class
        {
            SetAuthorizationHeader(_tokenProvider.GetAccessToken());

            var response = await _httpClient.GetAsync(uri);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new AuthenticationException(((int)response.StatusCode).ToString()); //Create custom exception
            }
            return JsonSerializer.Deserialize<TResult>(responseContent);

        }

        private async Task<T> GetAsync<T>(string uri) where T : class
        {
            SetAuthorizationHeader(_tokenProvider.GetAccessToken());

            var response = await _httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                throw new AuthenticationException(((int)response.StatusCode).ToString()); //Create custom exception
            }
            return await response.Content.ReadFromJsonAsync<T>();
        }

        private async Task DeleteAsync<T>(string uri) where T : class
        {
            SetAuthorizationHeader(_tokenProvider.GetAccessToken());

            var response = await _httpClient.DeleteAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                throw new AuthenticationException(((int)response.StatusCode).ToString()); //Create custom exception
            }
        }

        private void SetAuthorizationHeader(string accessToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
        #endregion
    }
}
