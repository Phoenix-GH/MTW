using System;
using System.Collections.Generic;
using System.Net;

namespace MyTenantWorld
{
	public class Legal
	{
		public string formName { get; set; }
		public string rulesandRegulation { get; set; }
		public string termsandCondition { get; set; }
		public bool status { get; set; }
	}

	public class Notification
	{
		public string emailTitle  { get; set; }
		public bool status { get; set; }
	}

	public class Indexing
	{
		public string format { get; set; }
		public string numberFormat { get; set; }
		public string numberPart1 { get; set; }
		public string numberPart2 { get; set; }
		public string numberPart3 { get; set; }
		public string numberPart4 { get; set; }
		public string numberPart5 { get; set; }
		public int minDigit { get; set; }
		public int initialNumber { get; set; }
	}
	
	public class Customization
	{
		public List<Legal> legal { get; set; }
		public List<Notification> notification { get; set; }
		public List<Indexing> indexing { get; set; }
		public string message { get; set; }
		public HttpStatusCode status_code { get; set; }
	}
}
