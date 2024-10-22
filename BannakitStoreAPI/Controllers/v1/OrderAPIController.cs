using AutoMapper;
using BannakitStore.Utility;
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
    public class OrderAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IOrderRepository _dbOrder;
        private readonly IMapper _mapper;

        public OrderAPIController(IOrderRepository dbOrder, IMapper mapper)
        {
            _dbOrder = dbOrder;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet, Route("allOrder")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetOrders([FromQuery] string search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Order> orderList;
                orderList = await _dbOrder.GetAllAsync(includeProperties: "Brand,Product,PaymentType", pageSize: pageSize, pageNumber: pageNumber);

                if (!string.IsNullOrEmpty(search))
                {
                    orderList = orderList.Where(u => u.OrderNo.Contains(search));
                }

                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageNumber };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Result = _mapper.Map<List<OrderDTO>>(orderList);
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

        [HttpGet, Route("getOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetOrder([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var stock = await _dbOrder.GetAsync(u => u.OrderId == id, tracked: true, includeProperties: "Brand,Product,PaymentType");

                if (stock == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }

                _response.Result = _mapper.Map<OrderDTO>(stock);
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
        [HttpPost, Route("saveOrder")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // 400 Bad Request(ข้อมูลไม่ถูกต้อง)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SaveOrder([FromBody] OrderEntryDTO createDTO)
        {
            try
            {
                if (createDTO == null)
                {
                    return BadRequest();
                }

                string newOrder = await _dbOrder.GenerateNewOrder();
                var message = Constants.PaymentStatus.GetMessage(createDTO.PaymentStatusId);

                Order order = _mapper.Map<Order>(createDTO);
                order.OrderNo = newOrder;
                order.OrderStatus = Constants.OrderStatus.PENDING;
                order.PaymentStatusTh = message.messageTh;
                order.PaymentStatusEn = message.messageEn;
                order.CreatedBy = "SYSTEM";
                order.CreatedDate = DateTime.Now;
                order.UpdatedBy = "SYSTEM";
                order.UpdatedDate = DateTime.Now;

                await _dbOrder.CreateAsync(order);

                _response.Result = _mapper.Map<OrderDTO>(order);
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
        [HttpPut, Route("updateOrder")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // 204 No Content(สำเร็จแต่ไม่ส่งข้อมูล)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // 400 Bad Request(ข้อมูลไม่ถูกต้อง)
        public async Task<ActionResult<APIResponse>> UpdateOrder(int id, [FromBody] OrderEntryDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.OrderId)
                {
                    return BadRequest();
                }

                var existingOrder = await _dbOrder.GetAsync(o => o.OrderId == id);
                if (existingOrder == null)
                {
                    return NotFound("Order not found");
                }

                var message = Constants.PaymentStatus.GetMessage(updateDTO.PaymentStatusId);

                existingOrder.UpdatedBy = "SYSTEM";
                existingOrder.UpdatedDate = DateTime.Now;
                existingOrder.ClientName = updateDTO.ClientName;
                existingOrder.ClientContact = updateDTO.ClientContact;
                existingOrder.Address = updateDTO.Address;
                existingOrder.BrandId = updateDTO.BrandId;
                existingOrder.ProdId = updateDTO.ProdId;
                existingOrder.Quatity = updateDTO.Quatity;
                existingOrder.TotalAmount = updateDTO.TotalAmount;
                existingOrder.AmountPaid = updateDTO.AmountPaid;
                existingOrder.PaymentTypeId = updateDTO.PaymentTypeId;
                existingOrder.PaymentStatusTh = message.messageTh;
                existingOrder.PaymentStatusEn = message.messageEn;
                existingOrder.Email = updateDTO.Email;
                existingOrder.OrderStatus = updateDTO.OrderStatus;
                existingOrder.DueAmount = updateDTO.DueAmount;

                await _dbOrder.UpdateAsync(existingOrder);
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

        [HttpDelete, Route("deleteOrder")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // 204 No Content(ลบสำเร็จ)
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]  // 404 Not Found(ไม่พบข้อมูล)
        [ProducesResponseType(StatusCodes.Status400BadRequest)]    // 400 Bad Request(คำร้องไม่ถูกต้อง)
        public async Task<ActionResult<APIResponse>> DeleteOrder([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var order = await _dbOrder.GetAsync(o => o.OrderId == id);

                if (order == null)
                {
                    return NotFound();
                }

                await _dbOrder.RemoveAsync(order);
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
