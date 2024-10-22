using AutoMapper;
using BannakitStoreApi.Models;
using BannakitStoreApi.Models.Dto;
using BannakitStoreApi.Reponsitory.IReponsitory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace BannakitStoreApi.Controllers.v1
{
    [Route("api/v{version:apiVersion}/categoryAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoryAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ICategoryRepository _dbCategory; // field
        private readonly IMapper _mapper; // field

        public CategoryAPIController(ICategoryRepository dbCategory, IMapper mapper)
        {
            _dbCategory = dbCategory;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet, Route("allCategories")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetCategories([FromQuery] string search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Category> categoryList;
                categoryList = await _dbCategory.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

                if (!string.IsNullOrEmpty(search))
                {
                    categoryList = categoryList.Where(u => u.CategoryNameTh.Contains(search) && u.CategoryNameEn.Contains(search));
                }

                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageSize };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Result = _mapper.Map<List<CategoryDTO>>(categoryList);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }

            return _response;
        }

        [HttpGet, Route("getCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCategory([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var category = await _dbCategory.GetAsync(u => u.CategoryId == id);
                if (category == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<CategoryDTO>(category);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;
        }

        [HttpPost, Route("saveCategory")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SaveCategory([FromBody] CategoryCreateDTO createDTO)
        {
            try
            {
                if (await _dbCategory.GetAsync(u => u.CategoryNameTh == createDTO.CategoryNameTh.ToLower() && u.CategoryNameEn.ToLower() == createDTO.CategoryNameEn.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Category already Exist!");
                    return BadRequest(ModelState);
                }

                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }

                Category category = _mapper.Map<Category>(createDTO);

                category.CreatedBy = "SYSTEM";
                category.CreatedDate = DateTime.Now;
                category.UpdatedBy = "SYSTEM";
                category.UpdatedDate = DateTime.Now;

                await _dbCategory.CreateAsync(category);
                //_response.Result = _mapper.Map<List<CategoryDTO>>(category);
                _response.Result = _mapper.Map<CategoryDTO>(category);
                _response.StatusCode = HttpStatusCode.Created;
                return _response;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpDelete, Route("deleteCategory")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteCategory(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var category = await _dbCategory.GetAsync(u => u.CategoryId == id);
                if (category == null)
                {
                    return NotFound();
                }

                await _dbCategory.RemoveAsync(category);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpPut, Route("updateCategory")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateCategory(int id, [FromBody] CategoryUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.CategoryId)
                {
                    return BadRequest();
                }

                Category model = _mapper.Map<Category>(updateDTO);

                await _dbCategory.UpdateAsync(model);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        //[HttpPatch]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> UpdatePartialCategory(int id, JsonPatchDocument<CategoryUpdateDTO> patchDTO)
        //{
        //    if (patchDTO == null || id == 0)
        //    {
        //        return BadRequest();
        //    }

        //    var category = await _dbCategory.GetAsync(u => u.CategoryId == id, tracked: false);
        //    CategoryUpdateDTO categoryDTO = _mapper.Map<CategoryUpdateDTO>(category);

        //    if (categoryDTO == null)
        //    {
        //        return BadRequest();
        //    }

        //    patchDTO.ApplyTo(categoryDTO, ModelState);
        //    Category model = _mapper.Map<Category>(categoryDTO);

        //    await _dbCategory.UpdateAsync(model);

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }

        //    return NoContent();
        //}
    }
}