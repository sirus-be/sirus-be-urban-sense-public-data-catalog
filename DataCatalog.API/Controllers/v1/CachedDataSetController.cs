using DataCatalog.API.Handlers;
using DataCatalog.API.Services;
using DataCatalog.Models;
using DataCatalog.Models.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace DataCatalog.API.Controllers.v1
{
    [Authorize]
    public class CachedDataSetController : BaseApiController<CachedDataSetController>
    {
        private readonly IDataSetService _dataSetService;
        private readonly IDistributedCache _distributedCache;

        public CachedDataSetController(IDataSetService dataSetService, IDistributedCache distributedCache)
        {
            _dataSetService = dataSetService ?? throw new ArgumentNullException(nameof(dataSetService));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        /// <summary>
        /// Returns a specific dataset distribution
        /// </summary>
        /// <param name="id">The dataset's id</param>
        /// <param name="distributionTitle">The distribution's title</param>
        /// <returns></returns>   
        [HttpGet]
        [AuthorizeRoles(Roles.SuperAdmin)]
        [Route("{id}/Distribution/{distributionTitle}")]
        public async Task<IActionResult> GetDataSetDistribution([FromRoute] string id, [FromRoute] string distributionTitle)
        {
            try
            {
                var cacheKey = $"Distribution-{id}-{distributionTitle}";
                var cachedKeyValues = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedKeyValues))
                {
                    var jsonObj = JsonConvert.DeserializeObject<Distribution>(cachedKeyValues);
                    return Ok(jsonObj);
                }
                else
                {
                    var dataset = await _dataSetService.GetDataSetAsync(id);
                    if (dataset == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        var distribution = dataset.Distribution.FirstOrDefault(d => d.Title == distributionTitle);

                        var jsonDistribution = JsonConvert.SerializeObject(distribution);
                        var cacheEntryOptions = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                            .SetAbsoluteExpiration(TimeSpan.FromSeconds(120));

                        await _distributedCache.SetStringAsync(cacheKey, jsonDistribution, cacheEntryOptions);

                        return Ok(distribution);
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }
    }
}
