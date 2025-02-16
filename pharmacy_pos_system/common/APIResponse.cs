using System.Net;

namespace pharmacy_pos_system.common
{
    public class APIResponse
    {
        public APIResponse()
        {
            Errors = new List<string>();
        }
        public bool Status { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public dynamic Data { get; set; }
        public List<string> Errors { get; set; }
    }
}
