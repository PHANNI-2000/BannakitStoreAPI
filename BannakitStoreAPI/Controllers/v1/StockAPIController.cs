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
    public class StockAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IStockRepository _dbStock;
        private readonly IMapper _mapper;

        public StockAPIController(IStockRepository dbStock, IMapper mapper)
        {
            _dbStock = dbStock;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet, Route("allStock")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetStocks([FromQuery] string search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Stock> stockList;
                stockList = await _dbStock.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

                if (!string.IsNullOrEmpty(search))
                {
                    stockList = stockList.Where(u => u.ProdId.ToString().Contains(search) && u.BrandId.ToString().Contains(search));
                }

                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageNumber };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Result = _mapper.Map<List<StockDTO>>(stockList);
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

        [HttpGet, Route("getStock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetStock([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var stock = await _dbStock.GetAsync(u => u.StockId == id);

                if (stock == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }

                _response.Result = _mapper.Map<StockDTO>(stock);
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
        [HttpPost, Route("saveStock")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // 400 Bad Request(ข้อมูลไม่ถูกต้อง)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SaveStock([FromBody] StockDTO createDTO)
        {
            try
            {
                if (createDTO == null)
                {
                    return BadRequest();
                }

                Stock stock = _mapper.Map<Stock>(createDTO);
                stock.CreatedBy = "SYSTEM";
                stock.CreatedDate = DateTime.Now;
                stock.UpdatedBy = "SYSTEM";
                stock.UpdatedDate = DateTime.Now;

                await _dbStock.CreateAsync(stock);

                _response.Result = _mapper.Map<StockDTO>(stock);
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
        [HttpPut, Route("updateStock")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // 204 No Content(สำเร็จแต่ไม่ส่งข้อมูล)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // 400 Bad Request(ข้อมูลไม่ถูกต้อง)
        public async Task<ActionResult<APIResponse>> UpdateStock(int id, [FromBody] StockEntryDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.StockId)
                {
                    return BadRequest();
                }

                var existingStock = await _dbStock.GetAsync(u => u.StockId == id);
                if (existingStock == null)
                {
                    return NotFound("Stock not found");
                }

                existingStock.UpdatedBy = "SYSTEM";
                existingStock.UpdatedDate = DateTime.Now;
                existingStock.ProdId = updateDTO.ProdId;
                existingStock.BrandId = updateDTO.BrandId;
                existingStock.Quatity = updateDTO.Quatity;

                await _dbStock.UpdateAsync(existingStock);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;
        }

        [HttpDelete, Route("deleteStock")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // 204 No Content(ลบสำเร็จ)
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]  // 404 Not Found(ไม่พบข้อมูล)
        [ProducesResponseType(StatusCodes.Status400BadRequest)]    // 400 Bad Request(คำร้องไม่ถูกต้อง)
        public async Task<ActionResult<APIResponse>> DeleteStock([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var stock = await _dbStock.GetAsync(u => u.StockId == id);

                if (stock == null)
                {
                    return NotFound();
                }

                await _dbStock.RemoveAsync(stock);
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
