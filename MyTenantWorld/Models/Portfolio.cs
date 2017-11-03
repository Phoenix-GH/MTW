namespace MyTenantWorld
{
	public class Portfolio
    {
		public string pid { get; set; }
		public string portfolioName { get; set; }
		public string portfolioImage { get; set; }
		public string userGroup { get; set; }
		public string userGroupGuid { get; set; }
		public int pendingFeedback { get; set; }
		public int pendingApplication { get; set; }
		public int unpaidBooking { get; set; }
    }
}
