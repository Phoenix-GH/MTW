using System.Collections.Generic;

namespace MyTenantWorld
{
	public class BlockItem
	{
		public string blockNo { get; set; }
	}

    public class BlockItemWithSelection : BlockItem
	{
        public bool isSelected { get; set; }
	}
	
    public class BaseNews
	{
		public string title { get; set; }
		public string message { get; set; }
		public string visibility { get; set; }
		public string imagePath { get; set; }
		public bool notiGenerated { get; set; }
	}

    public class NewsRequest : BaseNews
    {
		public string effectiveStartDate { get; set; }
		public string effectiveEndDate { get; set; }
        public List<BlockItem> blockList { get; set; }
    }

    public class News : BaseNews
	{
		public string id { get; set; }
        public bool isSelected { get; set; }
        public string blockList { get; set; }
		public string effectivePeriod { get; set; }
	}
}
