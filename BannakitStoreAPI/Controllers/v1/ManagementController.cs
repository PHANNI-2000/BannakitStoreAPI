using AutoMapper;
using BannakitStoreApi.Models;
using BannakitStoreApi.Models.Dto;
using BannakitStoreApi.Reponsitory.IReponsitory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace BannakitStoreApi.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ManagementController : ControllerBase
    {
        private readonly IManagementRepository _dbManagement;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public ManagementController(IManagementRepository dbManagement, IMapper mapper)
        {
            _dbManagement = dbManagement;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet, Route("allManagement")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetManagements([FromQuery] string search, int pageNumber = 1, int pageSize = 0)
        {
            try
            {
                IEnumerable<Employee> employeeList;
                employeeList = await _dbManagement.GetAllAsync(includeProperties: "Department,User,Image", pageSize: pageSize, pageNumber: pageNumber);

                if (!string.IsNullOrEmpty(search))
                {
                    employeeList = employeeList.Where(emp => emp.FirstName.Contains(search) && emp.LastName.Contains(search));
                }

                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageNumber };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Result = _mapper.Map<List<ManagementDTO>>(employeeList);
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

        [HttpGet, Route("getManagement")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetManagement([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var management = await _dbManagement.GetAsync(u => u.EmployeeId == id);

                if (management == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }

                _response.Result = _mapper.Map<ManagementDTO>(management);
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

    }
}
