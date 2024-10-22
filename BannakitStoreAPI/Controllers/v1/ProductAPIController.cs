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
    [Route("api/v{version:apiVersion}/productAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ProductAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IProductRepository _dbProduct; // field
        private readonly IMapper _mapper; // field

        public ProductAPIController(IProductRepository dbProduct, IMapper mapper)
        {
            _dbProduct = dbProduct;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet, Route("allProduct")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetProducts([FromQuery] string search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Product> productList;
                productList = await _dbProduct.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

                if (!string.IsNullOrEmpty(search))
                {
                    productList = productList.Where(u => u.ProdNameTh.Contains(search) && u.ProdNameEn.Contains(search));
                }

                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageNumber };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Result = _mapper.Map<List<ProductDTO>>(productList);
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

        [HttpGet, Route("getProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetProduct([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var product = await _dbProduct.GetAsync(u => u.ProdId == id);

                if (product == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }

                _response.Result = _mapper.Map<ProductDTO>(product);
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

        [HttpPost, Route("saveProduct")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SaveProduct([FromBody] ProductCreateDTO createDTO)
        {
            try
            {
                if (await _dbProduct.GetAsync(u => u.ProdNameTh == createDTO.ProdNameTh || u.ProdNameEn.ToLower() == createDTO.ProdNameEn.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Product already Exist!");
                    return BadRequest(ModelState);
                }

                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }

                string newProdNo = await _dbProduct.GenerateNewProdNo();

                Product product = _mapper.Map<Product>(createDTO);

                product.ProdNo = newProdNo;
                product.CreatedBy = "SYSTEM";
                product.CreatedDate = DateTime.Now;
                product.UpdatedBy = "SYSTEM";
                product.UpdatedDate = DateTime.Now;

                await _dbProduct.CreateAsync(product);

                _response.Result = _mapper.Map<ProductDTO>(product);
                _response.StatusCode = HttpStatusCode.Created;
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

        [HttpDelete, Route("deleteProduct")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteProduct([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var product = await _dbProduct.GetAsync(u => u.ProdId == id);

                if (product == null)
                {
                    return NotFound();
                }

                await _dbProduct.RemoveAsync(product);
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

        [HttpPut, Route("updateProduct")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateProduct(int id, [FromBody] ProductUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.ProdId)
                {
                    return BadRequest();
                }

                var existingProduct = await _dbProduct.GetAsync(u => u.ProdId == id);
                if (existingProduct == null)
                {
                    return NotFound("Payment Type not found.");
                }

                //Product model = _mapper.Map<Product>(updateDTO);

                //model.UpdatedBy = "SYSTEM";
                //model.UpdatedDate = DateTime.Now;

                existingProduct.UpdatedBy = "SYSTEM";
                existingProduct.UpdatedDate = DateTime.Now;
                existingProduct.BrandId = updateDTO.BrandId ?? existingProduct.BrandId;
                existingProduct.CategoryId = updateDTO.CategoryId ?? existingProduct.CategoryId;
                existingProduct.ProdNameTh = updateDTO.ProdNameTh ?? existingProduct.ProdNameTh;
                existingProduct.ProdNameEn = updateDTO.ProdNameEn ?? existingProduct.ProdNameEn;
                existingProduct.Price = updateDTO.Price;
                existingProduct.Costprice = updateDTO.Costprice;
                existingProduct.ActiveStatus = updateDTO.ActiveStatus;
                existingProduct.DescTh = updateDTO.DescTh ?? existingProduct.DescTh;
                existingProduct.DescEn = updateDTO.DescEn ?? existingProduct.DescEn;
                existingProduct.Quatity = updateDTO.Quatity ?? existingProduct.Quatity;
                existingProduct.Available = updateDTO.Available ?? existingProduct.Available;
                existingProduct.Tax = updateDTO.Tax ?? existingProduct.Tax;
                existingProduct.Remark = updateDTO.Remark;

                await _dbProduct.UpdateAsync(existingProduct);
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
