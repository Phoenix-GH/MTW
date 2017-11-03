namespace MyTenantWorld
{
	public class BaseUnit
	{
		public string unitId { get; set; }
		public string unitNo { get; set; }
	}

	public class Unit : BaseUnit
	{
		public string blockNo { get; set; }
		public string floorNo { get; set; }
	}

	public class SearchUnit : Unit
	{
		public string userId { get; set; }
		public string userName { get; set; }
		public string userGroup { get; set; }
	}
}
