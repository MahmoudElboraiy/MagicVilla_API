using AutoMapper;
using MagicVilla_API.Data;
using MagicVilla_API.DTO;
using MagicVilla_API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public VillApiController(ApplicationDbContext db,IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<VillaDTO>))]
        public async Task<IActionResult> GetVillas()
        {
            IEnumerable<Villa> villas = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villas));
        }
        [HttpGet("id:int", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            { 

                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<VillaDTO>(villa));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO villaCreate)
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
            if (await _db.Villas.FirstOrDefaultAsync(v => v.Name.ToLower() == villaCreate.Name) != null)
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
            await _db.Villas.AddAsync(villa);
            await _db.SaveChangesAsync();
            return CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);
        }
        [HttpDelete("{id}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa =  await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO UpdateVilla)
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
             _db.Villas.Update(villa);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVilla(int id ,JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            Villa villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            if(villa== null)
            {
                return NotFound();
            }
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);
           
            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa villa2 = _mapper.Map<Villa>(villaDTO);
 
            _db.Villas.Update(villa2);
            await _db.SaveChangesAsync();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
         
            return NoContent();
        }
    }
}
