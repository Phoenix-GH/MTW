namespace MyTenantWorld
{
	public class PortfolioInfo :BaseResponse
    {
		public string id { get; set;}
		public string name { get; set; }
		public string image { get; set; }
		public string portfolioUrlName { get; set; }
		public string generalEnquiryContact { get; set; }
		public string maintenanceContact { get; set; }
		public string portfolioLogo { get; set; }
		public string portfolioLogoStoragePath { get; set; }
		public string portfolioWallpaperStoragePath { get; set; }
    }

	public class PortfolioInfoRequest
	{
		public string name { get; set; }
		public string image { get; set; }
		public string generalEnquiryContact { get; set; }
		public string maintenanceContact { get; set; }
		public string portfolioLogo { get; set; }
		public string portfolioUrlName { get; set; }
	}
}
