using DataCatalog.API.Services;
using DataCatalog.Models;
using DataCatalog.Models.Authorization;
using DataCatalog.Models.Data;
using DataCatalog.Models.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace DataCatalog.API.Controllers.v1
{
    [Authorize]
    public class RoleController : BaseApiController<RoleController>
    {
        private readonly IDataSetService _dataSetService;

        public RoleController(IDataSetService dataSetService)
        {
            _dataSetService = dataSetService ?? throw new ArgumentNullException(nameof(dataSetService));
        }

        /// <summary>
        /// Returns all roles
        /// </summary>
        /// <param name="roleParameters">Parameters for sorting, paging, filtering,...</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] RoleParameters roleParameters)
        {
            try
            {
                return Ok(await _dataSetService.GetAllRolesAsync(roleParameters));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }
        /// <summary>
        /// Return all data sets roles
        /// </summary>
        /// <param name="dataSetName">Dataset's name</param>
        /// <param name="roleParameters">Parameters for sorting, paging, filtering,...</param>
        /// <returns></returns>
        [HttpGet]
        [Route("DataSet/{dataSetName}")]
        public async Task<IActionResult> GetAllDataSetRoles([FromRoute] string dataSetName, [FromQuery]RoleParameters roleParameters)
        {
            try
            {
                return Ok(await _dataSetService.GetAllRolesAsync(dataSetName, roleParameters));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (AuthenticationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }


        /// <summary>
        /// Get one role by id
        /// </summary>
        /// <param name="id">The role's Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{name}")]
        [Produces("application/ld+json")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var role = await _dataSetService.GetRoleByNameAsync(name);

                if (role == null)
                    return NotFound();
                else
                    return Ok(role);
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
        /// Insert one role in the database
        /// </summary>
        /// <param name="role">The create role</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            try
            {
                var roleExists = await _dataSetService.GetRoleByNameAsync(role.Name);
                if (roleExists != null)
                    throw new ValidationException("Role already exists");

                var createdRole = await _dataSetService.CreateRoleAsync(role);

                return Ok(createdRole);
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
        /// Update a role from the database
        /// </summary>
        /// <param name="role">The update role</param>
        /// <returns></returns>
        [HttpPut()]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<ActionResult> Update([FromBody] UpdateRole role)
        {
            try
            {
                var updatedRole = await _dataSetService.UpdateRoleAsync(role);

                return Ok(updatedRole);
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
