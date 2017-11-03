using System.Net;

namespace MyTenantWorld
{
	public class BaseResponse
	{
		public string error { get; set; }
		public string error_description { get; set; }
		public string message { get; set; }
		public HttpStatusCode status_code { get; set; }
	}
}
