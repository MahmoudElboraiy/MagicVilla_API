using AutoMapper;
using MagicVilla_API.Data;
using MagicVilla_API.DTO;
using MagicVilla_API.Models;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillApiController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;

        public VillApiController(IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
            _response = new();
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<VillaDTO>))]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                IEnumerable<Villa> villas = await _dbVilla.GetAllAsync();
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
        [HttpGet("id:int", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    return NotFound();
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
                    return BadRequest(ModelState);
                }
                if (villaCreate == null)
                {
                    return BadRequest(villaCreate);
                }
                //if (villaDTO.Id > 0)
                //{
                //    return StatusCode(StatusCodes.Status500InternalServerError);
                //}
                if (await _dbVilla.GetAsync(v => v.Name.ToLower() == villaCreate.Name) != null)
                {
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
        [HttpDelete("{id}", Name = "DeleteVilla")]
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
                    return BadRequest(_response);
                }
                var villa = await _dbVilla.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    _response.HttpStatusCode = HttpStatusCode.NotFound;
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
        [HttpPut("{id}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO UpdateVilla)
        {
            try
            {
                if (id == 0 || id != UpdateVilla.Id)
                {
                    return BadRequest();
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
                return BadRequest();
            }
            Villa villa = await _dbVilla.GetAsync(v => v.Id == id, tracked: false);
            if (villa == null)
            {
                return NotFound();
            }
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa villa2 = _mapper.Map<Villa>(villaDTO);
            await _dbVilla.UpdateAsync(villa2);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }
    }
}
