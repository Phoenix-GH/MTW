using System.Collections.Generic;
using System.Net;

namespace MyTenantWorld
{
    public class AuditDay
    {
		public string eventPage { get; set; }
		public string eventDate { get; set; }
		public string time { get; set; }
		public string description { get; set; }
		public string action { get; set; }
    }

    public class AuditData : AuditDay
    {
        public string date { get; set; }
    }

    public class Audit: AuditData
	{	
		public string header { get; set; }

	}

    public class AuditDataCollectionItem : List<AuditDay>
    {
        public string date { get; set; }
        public string timeList { get; set; }
        public string descriptionList { get; set; }
    }

    public class AuditCollectionItem:List<AuditDataCollectionItem>
    {
        public string header { get; set; }
        public List<AuditDataCollectionItem> auditData => this;
    }
}
