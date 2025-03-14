using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/UsersAuth")]
    [ApiController]
    public class UsersController : Controller
    {

        private readonly IUserRepository _userRepo;
        protected APIResponse _response;
        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
            this._response = new();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userRepo.Login(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.HttpStatusCode = HttpStatusCode.BadRequest;
                _response.IsSucessed = false;
                _response.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_response);
            }
            _response.HttpStatusCode = HttpStatusCode.OK;
            _response.IsSucessed = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
        {
            bool ifUserNameUnique = _userRepo.IsUnique(model.UserName);
            if (!ifUserNameUnique)
            {
                _response.HttpStatusCode = HttpStatusCode.BadRequest;
                _response.IsSucessed = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }

            var user = await _userRepo.Register(model);
            if (user == null)
            {
                _response.HttpStatusCode = HttpStatusCode.BadRequest;
                _response.IsSucessed = false;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);
            }
            _response.HttpStatusCode = HttpStatusCode.OK;
            _response.IsSucessed = true;
            return Ok(_response);
        }
    }
}
