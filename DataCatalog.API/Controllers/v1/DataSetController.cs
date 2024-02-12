using DataCatalog.API.Handlers;
using DataCatalog.API.Services;
using DataCatalog.Models;
using DataCatalog.Models.Authorization;
using DataCatalog.Models.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace DataCatalog.API.Controllers.v1
{
    [Authorize]
    public class DataSetController : BaseApiController<DataSetController>
    {
        private readonly IDataSetService _dataSetService;

        public DataSetController(IDataSetService dataSetService)
        {
            _dataSetService = dataSetService ?? throw new ArgumentNullException(nameof(dataSetService));
        }

        /// <summary>
        /// Returns all datasets as json-ld in the database
        /// </summary>
        /// <param name="dataSetParameters">Parameters for sorting, paging, filtering,...</param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/ld+json")]
        public async Task<IActionResult> GetAll([FromQuery] DataSetParameters dataSetParameters)
        {
            try
            {
                return Ok(await _dataSetService.GetAllDataSetsAsync(dataSetParameters));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Get one dataset as json-ld by id from the database
        /// </summary>
        /// <param name="id">The dataset's ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Produces("application/ld+json")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var dataset = await _dataSetService.GetDataSetAsync(id);

                if (dataset == null)
                    return NotFound();
                else
                    return Ok(dataset);
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

        /// <summary>
        /// Returns a specific dataset distribution
        /// </summary>
        /// <param name="id">The dataset's id</param>
        /// <param name="distributionTitle">The distribution's title</param>
        /// <returns></returns>   
        [HttpGet]
        [Route("{id}/Distribution/{distributionTitle}")]
        public async Task<IActionResult> GetDataSetDistribution([FromRoute] string id, [FromRoute] string distributionTitle)
        {
            try
            {
                var dataset = await _dataSetService.GetDataSetAsync(id);
                if (dataset == null)
                {
                    return NotFound();
                }
                else
                {
                    var distribution = dataset.Distribution.FirstOrDefault(d => d.Title == distributionTitle);
                    return Ok(distribution);
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

        /// <summary>
        /// Insert one dataset in the database
        /// </summary>
        /// <param name="dataSet">The create dataset</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Writer, Roles.Admin, Roles.SuperAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LinkedDataSet dataSet)
        {
            try
            {
                var dataSetExists = await _dataSetService.GetDataSetAsync(dataSet.Id);
                if (dataSetExists != null)
                    return BadRequest("Dataset already exists");

                var createdDataSet = await _dataSetService.CreateDataSetAsync(dataSet);

                return Ok(createdDataSet);
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

        /// <summary>
        /// Update a dataset from the database
        /// </summary>
        /// <param name="id">The dataset's ID</param>
        /// <param name="dataSet">The update dataset</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Writer, Roles.Admin, Roles.SuperAdmin)]
        [HttpPut()]
        public async Task<ActionResult> Update([FromBody] LinkedDataSet dataSet)
        {
            try
            {
                var updatedDataSet = await _dataSetService.UpdateDataSetAsync(dataSet);
                if (updatedDataSet == null)
                    return BadRequest("Dataset does not exists");

                return Ok(updatedDataSet);
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

        /// <summary>
        /// Delete a dataset from the database
        /// </summary>
        /// <param name="id">The datasets's ID</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.SuperAdmin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _dataSetService.DeleteDataSetAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }
    }
}
