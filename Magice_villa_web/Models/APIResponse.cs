using System.Net;

namespace MagicVilla_API.Models
{
    public class APIResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public bool IsSucessed { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }
    }
}
