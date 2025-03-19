using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;

namespace MagicVilla_API.Repository.IRepository
{
    public interface IUserRepository
    {
        public bool IsUnique(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}
