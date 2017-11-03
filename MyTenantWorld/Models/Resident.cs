using System.Net;

namespace MyTenantWorld
{
	public class Resident
	{
		public string userId { get; set; }
		public string userName { get; set; }
		public string unitId { get; set; }
		public string userGroup { get; set; }
	}

	public class ResidentUser
	{
		public string userGroup { get; set; }
		public string email { get; set; }
	}

	public class SpecificResident : ResidentUser
	{
		public HttpStatusCode status_code { get; set; }
		public string screenName { get; set; }
		public string givenName { get; set; }
		public string familyName { get; set; }
		public bool status { get; set; }
	}
}
