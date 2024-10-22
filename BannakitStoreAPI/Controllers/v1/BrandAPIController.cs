using AutoMapper;
using BannakitStoreApi.Models;
using BannakitStoreApi.Models.Dto;
using BannakitStoreApi.Reponsitory.IReponsitory;
using Microsoft.AspNetCore.Authorization;
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
    public class BrandAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IBrandRepository _dbBrand; // field
        private readonly IMapper _mapper; // field

        public BrandAPIController(IBrandRepository dbBrand, IMapper mapper)
        {
            _dbBrand = dbBrand;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet, Route("allBrand")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetBrands([FromQuery] string search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Brand> brandList;
                brandList = await _dbBrand.GetAllAsync(includeProperties: "Category");

                if (!string.IsNullOrEmpty(search))
                {
                    brandList = brandList.Where(u => u.BrandNameTh.Contains(search) && u.BrandNameEn.Contains(search));
                }

                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageNumber };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Result = _mapper.Map<List<BrandDTO>>(brandList);
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

        [HttpGet, Route("getBrand")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetBrand([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var brand = await _dbBrand.GetAsync(u => u.BrandId == id);

                if (brand == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }

                _response.Result = _mapper.Map<BrandDTO>(brand);
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

        // 201 Created(สำเร็จ)
        // 409 Conflict(ข้อมูลซ้ำ)
        [HttpPost, Route("saveBrand")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // 400 Bad Request(ข้อมูลไม่ถูกต้อง)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SaveBrand([FromBody] BrandEntryDTO createDTO)
        {
            try
            {
                if (await _dbBrand.GetAsync(u => (u.BrandNameTh == createDTO.BrandNameTh || u.BrandNameEn == createDTO.BrandNameEn) && u.CategoryId == createDTO.CategoryId) != null)
                {
                    ModelState.AddModelError("CustomError", "Brand already Exist!");
                    return BadRequest();
                }

                if (createDTO == null)
                {
                    return BadRequest();
                }

                Brand brand = _mapper.Map<Brand>(createDTO);
                brand.CreatedBy = "SYSTEM";
                brand.CreatedDate = DateTime.Now;
                brand.UpdatedBy = "SYSTEM";
                brand.UpdatedDate = DateTime.Now;

                await _dbBrand.CreateAsync(brand);

                _response.Result = _mapper.Map<BrandDTO>(brand);
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
        [HttpPut, Route("updateBrand")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // 204 No Content(สำเร็จแต่ไม่ส่งข้อมูล)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // 400 Bad Request(ข้อมูลไม่ถูกต้อง)
        public async Task<ActionResult<APIResponse>> UpdateBrand(int id, [FromBody] BrandEntryDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.BrandId)
                {
                    return BadRequest();
                }

                var existingBrand = await _dbBrand.GetAsync(u => u.BrandId == id);
                if (existingBrand == null)
                {
                    return NotFound("Brand not found");
                }

                existingBrand.UpdatedBy = "SYSTEM";
                existingBrand.UpdatedDate = DateTime.Now;
                existingBrand.BrandNameTh = updateDTO.BrandNameTh ?? existingBrand.BrandNameTh;
                existingBrand.BrandNameEn = updateDTO.BrandNameEn ?? existingBrand.BrandNameEn;
                existingBrand.ActiveStatus = updateDTO.ActiveStatus;
                existingBrand.CategoryId = updateDTO.CategoryId;

                await _dbBrand.UpdateAsync(existingBrand);
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

        [HttpDelete, Route("deleteBrand")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // 204 No Content(ลบสำเร็จ)
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]  // 404 Not Found(ไม่พบข้อมูล)
        [ProducesResponseType(StatusCodes.Status400BadRequest)]    // 400 Bad Request(คำร้องไม่ถูกต้อง)
        public async Task<ActionResult<APIResponse>> DeleteBrand([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var brand = await _dbBrand.GetAsync(u => u.BrandId == id);

                if (brand == null)
                {
                    return NotFound();
                }

                await _dbBrand.RemoveAsync(brand);
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
