using System.Net;

namespace Magic_villa_web.Models
{
    public class APIResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public bool IsSucessed { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }
    }
}
