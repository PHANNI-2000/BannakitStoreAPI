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
    public class DepartmentAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IDepartmentRepository _dbDepartment;
        private readonly IMapper _mapper;

        public DepartmentAPIController(IDepartmentRepository dbDepartment, IMapper mapper)
        {
            _dbDepartment = dbDepartment;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet, Route("allDepartment")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetDepartments([FromQuery] string search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Department> departmentList;
                departmentList = await _dbDepartment.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

                if (!string.IsNullOrEmpty(search))
                {
                    departmentList = departmentList.Where(u => u.DeptNameTh.Contains(search) && u.DeptNameEn.Contains(search));
                }

                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageNumber };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Result = _mapper.Map<List<DepartmentDTO>>(departmentList);
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
        public async Task<ActionResult<APIResponse>> GetDepartment([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var department = await _dbDepartment.GetAsync(u => u.DepartmentId == id);

                if (department == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }

                _response.Result = _mapper.Map<DepartmentDTO>(department);
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

        [HttpPost, Route("saveDepartment")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // 400 Bad Request(ข้อมูลไม่ถูกต้อง)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SaveDepartment([FromBody] DepartmentDTO createDTO)
        {
            try
            {
                if (await _dbDepartment.GetAsync(u => u.DeptNameTh == createDTO.DeptNameTh && u.DeptNameEn == createDTO.DeptNameEn) != null)
                {
                    return BadRequest();
                }

                if (createDTO == null)
                {
                    return BadRequest();
                }

                Department department = _mapper.Map<Department>(createDTO);
                department.CreatedBy = "SYSTEM";
                department.CreatedDate = DateTime.Now;
                department.UpdatedBy = "SYSTEM";
                department.UpdatedDate = DateTime.Now;

                await _dbDepartment.CreateAsync(department);

                _response.Result = _mapper.Map<Department>(department);
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
        [HttpPut, Route("updateDepartment")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // 204 No Content(สำเร็จแต่ไม่ส่งข้อมูล)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // 400 Bad Request(ข้อมูลไม่ถูกต้อง)
        public async Task<ActionResult<APIResponse>> UpdateDepartment(int id, [FromBody] DepartmentEntryDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.DepartmentId)
                {
                    return BadRequest();
                }

                var existingDepartment = await _dbDepartment.GetAsync(u => u.DepartmentId == id);
                if (existingDepartment == null)
                {
                    return NotFound("Department not found");
                }

                existingDepartment.UpdatedBy = "SYSTEM";
                existingDepartment.UpdatedDate = DateTime.Now;
                existingDepartment.DeptNameTh = updateDTO.DeptNameTh ?? existingDepartment.DeptNameTh;
                existingDepartment.DeptNameEn = updateDTO.DeptNameEn ?? existingDepartment.DeptNameEn;
                existingDepartment.ActiveStatus = updateDTO.ActiveStatus;

                await _dbDepartment.UpdateAsync(existingDepartment);
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

        [HttpDelete, Route("deleteDepartment")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // 204 No Content(ลบสำเร็จ)
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]  // 404 Not Found(ไม่พบข้อมูล)
        [ProducesResponseType(StatusCodes.Status400BadRequest)]    // 400 Bad Request(คำร้องไม่ถูกต้อง)
        public async Task<ActionResult<APIResponse>> DeleteDepartment([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var department = await _dbDepartment.GetAsync(u => u.DepartmentId == id);

                if (department == null)
                {
                    return NotFound();
                }

                await _dbDepartment.RemoveAsync(department);
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
