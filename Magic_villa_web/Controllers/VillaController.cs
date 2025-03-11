using AutoMapper;
using Magic_villa_web.DTO;
using Magic_villa_web.Models;
using Magic_villa_web.Models.DTO;
using Magic_villa_web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Magic_villa_web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaController(IVillaService villaService,IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }
        [HttpGet]
        public async  Task<IActionResult> IndexVilla()
        {
            List<VillaDTO> list = new();
            var response =await _villaService.GetAllAsync<APIResponse>();
            if(response !=null && response.IsSucessed)
            {
                list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
            }
            return View(list);
        }
        [HttpGet]
        public async Task<IActionResult> CreateVilla()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVilla(VillaCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.CreateAsync<APIResponse>(model);
                if (response != null && response.IsSucessed)
                {
                    TempData["success"] = "Villa created successfully";
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
            TempData["error"] = "Error encountered.";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateVilla(int villaId)
        {
            var response = await _villaService.GetAsync<APIResponse>(villaId);
            if (response != null && response.IsSucessed)
            {
                VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));

                return View(_mapper.Map<VillaUpdateDTO>(model));
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVilla(VillaUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.UpdateAsync<APIResponse>(model);
                if (response != null && response.IsSucessed)
                {
                    TempData["success"] = "Villa updated successfully";
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
            TempData["error"] = "Error encountered.";
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteVilla(int villaId)
        {
            var response = await _villaService.GetAsync<APIResponse>(villaId);
            if (response != null && response.IsSucessed)
            {
                VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVilla(VillaDTO model)
        {

                var response = await _villaService.DeleteAsync<APIResponse>(model.Id);
                if (response != null && response.IsSucessed)
                {
                TempData["success"] = "Villa deleted successfully";
                return RedirectToAction(nameof(IndexVilla));
                }
            TempData["error"] = "Error encountered.";
            return View(model);
        }
    }
}
