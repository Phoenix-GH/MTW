using System.Collections.Generic;
using System.Net;

namespace MyTenantWorld
{
	public class FacilityItem
    {
		public string facilityId { get; set; }
		public string facilityName { get; set; }
		public string description { get; set; }
		public string photo { get; set; }
       
    }

	public class FacilityDetailRequest
	{
		public string name { get; set; }
		public string description { get; set; }
		public string photo { get; set; }
		public string type { get; set; }
		public int capacity { get; set; }
		public float deposit { get; set; }
		public string equipment { get; set; }
		public string contactPerson { get; set; }
		public string contactNumber { get; set; }
		public string contactFax { get; set; }
		public string contactEmail { get; set; }
		public string guideLines { get; set; }
		public bool status { get; set; }
		public string effectiveDate { get; set; }
		public string endDate { get; set; }
		public string locationMap { get; set; }	
	}

	public class Facility: FacilityRequest
	{
		public string id { get; set; }
		public string message { get; set; }
		public HttpStatusCode status_code { get; set; }
		public List<object> modalState { get; set; }
		public Dictionary<string, object> mis { get; set; }
	}

	public class FacilityRequest : FacilityDetailRequest
	{
		public List<FacilityTimeSlot> tFacilityTimeSlots { get; set; }
		public FacilityOtherSetting tFacilityOtherSetting { get; set; }
	}

    public class FacilityBookingInfo:BaseResponse
	{
		public string facilityName { get; set; }
		public float deposit { get; set; }
		public string photo { get; set; }
		public string blockNo { get; set; }
		public string unitNo { get; set; }
		public string userName { get; set; }
		public string effectiveStartDate { get; set; }
		public string effectiveEndDate { get; set; }
		public List<AvailableBookingTimeSlot> availableBookingTimeSlotList { get; set; }
        public List<AvailableBookingTimeSlot> reservedBookingTimeSlotList { get; set; }
	}
}
