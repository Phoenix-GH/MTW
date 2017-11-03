using System;

namespace MyTenantWorld
{
	public class FacilityTimeSlot
    {
		public string slotDay { get; set;}
		public string startTime { get; set; }
		public string endTime { get; set; }
		public float slotRate { get; set; }
		public string slotType { get; set; }
       
    }

	public class FacilityOtherSetting 
	{
		public int advance_Booking_Max { get; set; }
		public string advance_Booking_MaxType { get; set; }
		public int advance_Booking_Min { get; set; }
		public string advance_Booking_MinType { get; set; }
		public int cancel_Booking { get; set; }
		public string cancel_BookingType { get; set; }
		public int max_Booking { get; set; }
		public string max_BookingType { get; set; }
		public int max_Prime_Booking { get; set; }
		public string max_Prime_BookingType { get; set; }
		public int slot_Limit { get; set; }
		public bool allow_Consecutive { get; set; }
		public bool booking_Same { get; set; }
		public string booking_SameType { get; set; }
		public int auto_Cancellation { get; set; }
		public string auto_CancellationType { get; set; }
		public bool enable_Max_Booking { get; set; }
		public bool max_BookingWithinGrp { get; set; }
		public bool max_Prime_BookingWithinGrp { get; set; }
		public bool booking_SameWithinGrp { get; set; }
	}

	public class FacilityTimeSlotCellModel 
	{
		public TimeSpan startTime { get; set; }
		public TimeSpan endTime { get; set; }
		public string slotRateText { get; set; }
		public bool slotTypeToggle { get; set; }
		public string slotDay { get; set; }
	}

	public class AvailableBookingTimeSlot : FacilityTimeSlot
	{
		public string tId { get; set;}
		public bool status { get; set; }
		public bool notified { get; set; }
	}
}
