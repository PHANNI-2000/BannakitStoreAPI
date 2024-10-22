using AutoMapper;
using BannakitStoreApi.Models;
using BannakitStoreApi.Models.Dto;
using BannakitStoreApi.Reponsitory.IReponsitory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;

namespace BannakitStoreApi.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class RoleAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IRoleRepository _dbRole;
        private readonly IMapper _mapper;

        public RoleAPIController(IRoleRepository dbRole, IMapper mapper)
        {
            _dbRole = dbRole;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet, Route("allRole")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetRoles([FromQuery] string search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Role> roleList;
                roleList = await _dbRole.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

                if (!string.IsNullOrEmpty(search))
                {
                    roleList = roleList.Where(u => u.RoleNameTh.Contains(search) && u.RoleNameEn.Contains(search));
                }

                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageNumber };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Result = _mapper.Map<List<RoleDTO>>(roleList);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message.ToString() };
            }

            return _response;
        }

        [HttpGet, Route("getRole")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetRole([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var role = await _dbRole.GetAsync(u => u.RoleId == id);

                if (role == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }

                _response.Result = _mapper.Map<RoleDTO>(role);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;
        }

        [HttpPost, Route("saveRole")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // 400 Bad Request(ข้อมูลไม่ถูกต้อง)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SaveRole([FromBody] RoleDTO createDTO)
        {
            try
            {
                if (await _dbRole.GetAsync(u => u.RoleNameTh == createDTO.RoleNameTh && u.RoleNameEn == createDTO.RoleNameEn) != null)
                {
                    return BadRequest();
                }

                if (createDTO == null)
                {
                    return BadRequest();
                }

                Role role = _mapper.Map<Role>(createDTO);
                role.CreatedBy = "SYSTEM";
                role.CreatedDate = DateTime.Now;
                role.UpdatedBy = "SYSTEM";
                role.UpdatedDate = DateTime.Now;

                await _dbRole.CreateAsync(role);

                _response.Result = _mapper.Map<RoleDTO>(role);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }

            return _response;
        }

        // 200 OK(สำเร็จพร้อมส่งข้อมูล)
        // 404 Not Found(ไม่พบข้อมูล)
        [HttpPut, Route("updateRole")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // 204 No Content(สำเร็จแต่ไม่ส่งข้อมูล)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // 400 Bad Request(ข้อมูลไม่ถูกต้อง)
        public async Task<ActionResult<APIResponse>> UpdateRole(int id, [FromBody] RoleEntryDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.RoleId)
                {
                    return BadRequest();
                }

                var existingRole = await _dbRole.GetAsync(u => u.RoleId == id);
                if (existingRole == null)
                {
                    return NotFound("Role not found");
                }

                existingRole.UpdatedBy = "SYSTEM";
                existingRole.UpdatedDate = DateTime.Now;
                existingRole.RoleNameTh = updateDTO.RoleNameTh ?? existingRole.RoleNameTh;
                existingRole.RoleNameEn = updateDTO.RoleNameEn ?? existingRole.RoleNameEn;

                await _dbRole.UpdateAsync(existingRole);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent; // สำเร็จแต่ไม่ส่งข้อมูล

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;
        }

        [HttpDelete, Route("deleteRole")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // 204 No Content(ลบสำเร็จ)
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]  // 404 Not Found(ไม่พบข้อมูล)
        [ProducesResponseType(StatusCodes.Status400BadRequest)]    // 400 Bad Request(คำร้องไม่ถูกต้อง)
        public async Task<ActionResult<APIResponse>> DeleteRole([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var role = await _dbRole.GetAsync(u => u.RoleId == id);

                if (role == null)
                {
                    return NotFound();
                }

                await _dbRole.RemoveAsync(role);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;
        }
    }
}
