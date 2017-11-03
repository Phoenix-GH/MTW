using System.Collections.Generic;

namespace MyTenantWorld
{
	public class HomeProfile : BaseResponse
    {
		public List<Portfolio> staffPortfolioList { get; set;}
		public string userId { get; set;}
		public string screenName { get; set; }
		public string defaultPid { get; set; }
		public string userRole { get; set; }
		public string azureStorageAccConnString { get; set; }
        public int pendingFeedback { get; set; }
        public int pendingApplication { get; set; }
        public int unpaidBooking { get; set; }
    }
}
