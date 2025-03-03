using AutoMapper;
using MagicVilla_API.DTO;
using MagicVilla_API.Models;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaNumberController : ControllerBase
    {
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IMapper _mapper;
        public APIResponse _response;

        public VillaNumberController(IVillaNumberRepository dbVillaNumber,IMapper mapper)
        {
            _dbVillaNumber = dbVillaNumber;
            _mapper = mapper;
            _response = new();
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllVillasNumber()
        {
            try
            {
                IEnumerable<VillaNumber> villasNumber = await _dbVillaNumber.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villasNumber);
                _response.HttpStatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }catch(Exception ex)
            {
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                _response.IsSucessed = false;
            }
            return _response;
        }
        [HttpGet("{villaNo:int}", Name ="GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNo)
        {
            try
            {
                VillaNumber villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == villaNo);
                if(villaNumber == null)
                {
                    _response.HttpStatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.HttpStatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                _response.IsSucessed = false;
            }
            return _response;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber ([FromBody]VillaNumberCreateDTO villaNumber)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                //if (await _dbVillaNumber.GetAsync(v => v.VillaNo == villaNumber.VillaNo) != null)
                //{
                //    ModelState.AddModelError("", "the number must be unique");
                //    return BadRequest(ModelState);
                //}
                VillaNumber villanumber1 = _mapper.Map<VillaNumber>(villaNumber);
                villanumber1.CreateDate = DateTime.Now;
                villanumber1.UpdateDate = DateTime.Now;
                await _dbVillaNumber.CreateAsync(villanumber1);
                _response.Result = _mapper.Map<VillaNumberDTO>(villanumber1);
                _response.HttpStatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVillaNumber", new { villano = villanumber1.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                _response.IsSucessed = false;
            }
            return _response;
        }
        [HttpDelete("{villaNo:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async  Task<ActionResult<APIResponse>> DeleteVillaNumber (int villaNo)
        {
            try
            { 
            if (villaNo < 0)
            {
                 _response.HttpStatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            VillaNumber villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == villaNo);
            if(villaNumber == null)
            {
                 _response.HttpStatusCode = HttpStatusCode.NotFound;
                 return NotFound(_response);
            }
            await _dbVillaNumber.RemoveAsync(villaNumber);
            _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
            _response.HttpStatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                _response.IsSucessed = false;
            }
            return _response;
        }
        [HttpPut("{villaNo:int}")]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo ,[FromBody]VillaNumberUpdateDTO villaUpdateDTO) 
        {
            try
            {
                if (villaNo < 0)
                {
                    _response.HttpStatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                VillaNumber villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == villaNo,tracked:false);
                if (villaNumber == null)
                {
                    _response.HttpStatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                VillaNumber villa =_mapper.Map<VillaNumber>(villaUpdateDTO);
                await _dbVillaNumber.UpdateAsync(villa);
                _response.Result = _mapper.Map<VillaNumberDTO>(villa); ;
                _response.HttpStatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                _response.IsSucessed = false;
            }
            return _response;
        }
    }
}
