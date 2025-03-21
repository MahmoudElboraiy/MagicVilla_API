﻿using AutoMapper;
using MagicVilla_API.DTO;
using MagicVilla_API.Models;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/VillaNumber")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaNumberController : ControllerBase
    {
        private readonly IVillaNumberRepository _dbVillaNumber;

        private readonly IMapper _mapper;
        private readonly IVillaRepository _dbVilla;
        protected APIResponse _response;
        public VillaNumberController(IVillaNumberRepository dbVillaNumber,IMapper mapper,IVillaRepository dbVilla)
        {
            _dbVillaNumber = dbVillaNumber;
            _mapper = mapper;
            _dbVilla = dbVilla;
            _response = new();
        }
        [HttpGet("GetString")]
        public IEnumerable<string> Get()
        {
            return new string[] { "String1", "string2" };
        }
        //[MapToApiVersion("1.0")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllVillasNumber()
        {
            try
            {
                IEnumerable<VillaNumber> villasNumber = await _dbVillaNumber.GetAllAsync(includeProperties:"Villa");
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
                    _response.IsSucessed = false;
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

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber ([FromBody]VillaNumberCreateDTO villaNumber)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSucessed = false;
                    return BadRequest(ModelState);
                }
                if (await _dbVillaNumber.GetAsync(v => v.VillaNo == villaNumber.VillaNo) != null)
                {
                    _response.IsSucessed = false;
                    ModelState.AddModelError("", "the number must be unique");
                    return BadRequest(ModelState);
                }
                if(await _dbVilla.GetAsync(v => v.Id == villaNumber.VillaId) == null)
                {
                    _response.IsSucessed = false;
                    ModelState.AddModelError("", "the villa id is not exits");
                    return BadRequest(ModelState);
                }
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
        [Authorize(Roles = "admin")]
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
                    _response.IsSucessed = false;
                    _response.HttpStatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            VillaNumber villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == villaNo);
            if(villaNumber == null)
            {
                 _response.HttpStatusCode = HttpStatusCode.NotFound;
                    _response.IsSucessed = false;
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
        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id ,[FromBody]VillaNumberUpdateDTO updateDTO) 
        {
            try
            {
                if (updateDTO == null || id != updateDTO.VillaNo)
                {
                    return BadRequest();
                }
                if (await _dbVilla.GetAsync(u => u.Id == updateDTO.VillaId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is Invalid!");
                    return BadRequest(ModelState);
                }
                VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);

                await _dbVillaNumber.UpdateAsync(model);
                _response.HttpStatusCode = HttpStatusCode.NoContent;
                _response.IsSucessed = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSucessed = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
