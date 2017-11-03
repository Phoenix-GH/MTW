using System.Collections.Generic;

namespace MyTenantWorld
{
    public class Committee
    {
        public string name { get; set; }
        public string description { get; set; }
        public int sortOrder { get; set; }
    }

    public class CommitteeRequest
    {
        public List<Committee> Committee { get; set; }        
    }
}
