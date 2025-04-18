﻿using AutoMapper;
using MagicVilla_API.Data;
using MagicVilla_API.DTO;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace MagicVilla_API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaApiController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;

        public VillaApiController(IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
            _response = new();
        }
        [HttpGet]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<VillaDTO>))]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "filterOccupancy")] int? occupancy
            , [FromQuery] string? search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Villa> villas;

                if (occupancy > 0)
                {
                    villas = await _dbVilla.GetAllAsync(u => u.Occupancy == occupancy, pageSize: pageSize,
                          pageNumber: pageNumber);
                }
                else
                {
                    villas = await _dbVilla.GetAllAsync(pageSize: pageSize,
                      pageNumber: pageNumber);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    villas = villas.Where(u => u.Name.ToLower().Contains(search));
                }
                Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
                _response.Result = _mapper.Map<List<VillaDTO>>(villas);
                _response.HttpStatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSucessed = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ResponseCache(Location =ResponseCacheLocation.None,NoStore =true)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSucessed = false;
                    return BadRequest(_response);
                }
                var villa = await _dbVilla.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    _response.IsSucessed = false;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.HttpStatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSucessed = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO villaCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSucessed = false;
                    return BadRequest(ModelState);
                }
                if (villaCreate == null)
                {
                    _response.IsSucessed = false;
                    return BadRequest(villaCreate);
                }

                //if (villaDTO.Id > 0)
                //{
                //    return StatusCode(StatusCodes.Status500InternalServerError);
                //}

                if (await _dbVilla.GetAsync(v => v.Name.ToLower() == villaCreate.Name) != null)
                {
                    _response.IsSucessed = false;
                    ModelState.AddModelError("", "the name must be unique");
                    return BadRequest(ModelState);
                }
                Villa villa = _mapper.Map<Villa>(villaCreate);

                //Villa villa = new Villa
                //{
                //    Name = villaCreate.Name,
                //    Sqft = villaCreate.Sqft,
                //    Occupancy = villaCreate.Occupancy,
                //    Rate = villaCreate.Rate,
                //    Details = villaCreate.Details,
                //    Amenity = villaCreate.Amenity,
                //    ImageUrl = villaCreate.ImageUrl,

                //};
                await _dbVilla.CreateAsync(villa);
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.HttpStatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSucessed = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.HttpStatusCode = HttpStatusCode.BadRequest;
                    _response.IsSucessed = false;
                    return BadRequest(_response);
                }
                var villa = await _dbVilla.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    _response.HttpStatusCode = HttpStatusCode.NotFound;
                    _response.IsSucessed = false;
                    return NotFound(_response);
                }
                await _dbVilla.RemoveAsync(villa);
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.HttpStatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSucessed = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO UpdateVilla)
        {
            try
            {
                if (id == 0 || id != UpdateVilla.Id)
                {
                    _response.IsSucessed = false;
                    return BadRequest(_response);
                }
                Villa villa = _mapper.Map<Villa>(UpdateVilla);
                //Villa villa = new Villa
                //{
                //    Id= UpdateVilla.Id,
                //    Name = UpdateVilla.Name,
                //    Sqft = UpdateVilla.Sqft,
                //    Occupancy = UpdateVilla.Occupancy,
                //    Rate = UpdateVilla.Rate,
                //    Details = UpdateVilla.Details,
                //    Amenity = UpdateVilla.Amenity,
                //    ImageUrl = UpdateVilla.ImageUrl,

                //};
                await _dbVilla.UpdateAsync(villa);
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.HttpStatusCode = HttpStatusCode.NoContent;
                _response.IsSucessed = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSucessed = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPatch("{id}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (id == 0)
            {
                _response.IsSucessed = false;
                return BadRequest(_response);
            }
            Villa villa = await _dbVilla.GetAsync(v => v.Id == id, tracked: false);
            if (villa == null)
            {
                _response.IsSucessed = false;
                return NotFound(_response);
            }
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa villa2 = _mapper.Map<Villa>(villaDTO);
            await _dbVilla.UpdateAsync(villa2);
            if (!ModelState.IsValid)
            {
                _response.IsSucessed = false;
                return BadRequest(ModelState);
            }

            return NoContent();
        }
    }
}
