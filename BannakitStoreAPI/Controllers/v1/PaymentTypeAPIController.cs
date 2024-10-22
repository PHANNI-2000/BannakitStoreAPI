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
    public class PaymentTypeAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IPaymentTypeRepository _dbPaymentType;
        private readonly IMapper _mapper;

        public PaymentTypeAPIController(IPaymentTypeRepository dbPaymentType, IMapper mapper)
        {
            _dbPaymentType = dbPaymentType;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet, Route("allPaymentType")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetPaymentTypes([FromQuery] string search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<PaymentType> paymenttypeList;
                paymenttypeList = await _dbPaymentType.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

                if (!string.IsNullOrEmpty(search))
                {
                    paymenttypeList = paymenttypeList.Where(u => u.PaymentNameTh.Contains(search) && u.PaymentNameEn.Contains(search));
                }

                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageSize };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Result = _mapper.Map<List<PaymentTypeDTO>>(paymenttypeList);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }

            return _response;
        }

        [HttpGet, Route("getPaymentType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetPaymentTpye([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var paymentType = await _dbPaymentType.GetAsync(u => u.PaymentTypeId == id);

                if (paymentType == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }

                _response.Result = _mapper.Map<PaymentTypeDTO>(paymentType);
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

        [HttpPost, Route("savePaymentType")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SavePaymentType([FromBody] PaymentTypeDTO createDTO)
        {
            try
            {
                if (await _dbPaymentType.GetAsync(u => u.PaymentNameTh == createDTO.PaymentNameTh || u.PaymentNameEn.ToLower() == createDTO.PaymentNameEn) != null)
                {
                    ModelState.AddModelError("CustomError", "Payment Type already Exist!");
                    return BadRequest();
                }

                if (createDTO == null)
                {
                    return BadRequest();
                }

                PaymentType paymentType = _mapper.Map<PaymentType>(createDTO);
                paymentType.CreatedBy = "SYSTEM";
                paymentType.CreatedDate = DateTime.Now;
                paymentType.UpdatedBy = "SYSTEM";
                paymentType.UpdatedDate = DateTime.Now;

                await _dbPaymentType.CreateAsync(paymentType);

                _response.Result = _mapper.Map<PaymentTypeDTO>(paymentType);
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

        [HttpDelete, Route("deletePaymentType")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeletePaymentType([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var paymentType = await _dbPaymentType.GetAsync(u => u.PaymentTypeId == id);

                if (paymentType == null)
                {
                    return NotFound();
                }

                await _dbPaymentType.RemoveAsync(paymentType);
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

        [HttpPut, Route("updatePaymentType")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdatePaymentType(int id, [FromBody] PaymentTypeEntryDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.PaymentTypeId)
                {
                    return BadRequest();
                }

                var existingPaymentType = await _dbPaymentType.GetAsync(u => u.PaymentTypeId == id);
                if (existingPaymentType == null)
                {
                    return NotFound("Payment Type not found.");
                }

                existingPaymentType.UpdatedBy = "SYSTEM";
                existingPaymentType.UpdatedDate = DateTime.Now;
                existingPaymentType.PaymentNameTh = updateDTO.PaymentNameTh;
                existingPaymentType.PaymentNameEn = updateDTO.PaymentNameEn;
                existingPaymentType.ActiveStatus = updateDTO.ActiveStatus;

                //PaymentType model = _mapper.Map<PaymentType>(updateDTO);

                await _dbPaymentType.UpdateAsync(existingPaymentType);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;

                return Ok(_response);
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
