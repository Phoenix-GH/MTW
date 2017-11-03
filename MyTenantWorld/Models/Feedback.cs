using System.Collections.Generic;

namespace MyTenantWorld
{
    public class Feedback
    {
		public string id { get; set; }
		public string subject { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public string feedbackDate { get; set; }
        public string _feedbackDate { get; set; }
		public string resolvedDate { get; set; }
		public string _resolvedDate { get; set; }
        public string residentName { get; set; }
        public string unitNo { get; set; }
    }

	public class FeedbackDetail
	{
		public string id { get; set; }
		public string reportedOn { get; set; }
		public string _reportedOn { get; set; }
        public string reporter { get; set; }
		public string subject { get; set; }
        public string description { get; set; }
		public string status { get; set; }
		public string type { get; set; }
        public string replyRemark { get; set; }
		public string repliedOn { get; set; }
		public string _repliedOn { get; set; }
        public List<FeedbackDetailImage> tFeedBackDetails { get; set; }
	}

    public class FeedbackDetailImage
    {
        public string imagePath { get; set; }
    }

	public class UpdateFeedbackRequest
	{
		public string status { get; set; }
        public string replyremark { get; set; }
	}

    public class UpdateFeedbackResponse : UpdateFeedbackRequest
	{
		public string id { get; set; }
	}
}
