namespace MyTenantWorld
{
	public class TenantID
	{
		public string tenantId { get; set; }
		
	}
    public class Tenant:TenantID
	{
	
		public string tenantName { get; set; }
	}
}
