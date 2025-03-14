using System.Net;

namespace MagicVilla_API.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }
        public HttpStatusCode HttpStatusCode { get; set; }
        public bool IsSucessed { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }
    }
}
