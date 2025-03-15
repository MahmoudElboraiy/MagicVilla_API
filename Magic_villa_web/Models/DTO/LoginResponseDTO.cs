using Magic_villa_web.Models.DTO;

namespace Magic_villa_web.DTO
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}
