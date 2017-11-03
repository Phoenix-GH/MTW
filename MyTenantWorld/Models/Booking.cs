using System.Collections.Generic;

namespace MyTenantWorld
{
	public class BookingRequest
	{
		public bool bookingconfirm { get; set; }
		public List<ConfirmedBookingTimeSlot> confirmedBookingTimeSlotList { get; set; }
	}

	public class ConfirmedBookingTimeSlot
	{
		public string tid { get; set; }
	}

    public class NotifyResidentRequest
    {
        public List<ConfirmedBookingTimeSlot> waitingBookingTimeSlotList { get; set; }
    }

    public class BookingDetails:BaseResponse
	{
		public string bookingID { get; set; }
		public float totalPayment { get; set; }
		public float bookingFee { get; set; }
		public float depositFee { get; set; }
        public string remarks { get; set; }
        public string paymentMethod { get; set; }
        public string receiptNo { get; set; }
        public List<AvailableBookingTimeSlot> confirmedBookingTimeSlotList { get; set; }
        public List<Tenant> reminderList { get; set; }
	}

    public class ReservedBookingRequest
    {
		public string remarks { get; set; }
		public bool bookingconfirm { get; set; }
		public string paymentMethod { get; set; }
		public List<ConfirmedBookingTimeSlot> confirmedBookingTimeSlotList { get; set; }
    }

    public class PaidBookingRequest:ReservedBookingRequest
    {
        public List<Tenant> reminderList { get; set; }
    }

    public class PaidBookingResponse:BaseResponse
    {
		public string bookingID { get; set; }
		public float totalPayment { get; set; }
		public float bookingFee { get; set; }
		public float depositFee { get; set; }
		public string remarks { get; set; }
		public string paymentMethod { get; set; }
		public bool bookingConfirm { get; set; }
    }

	public class BookingResponse : BaseResponse
	{
		public string bookingID { get; set; }
		public string remarks { get; set; }
		public string paymentMethod { get; set; }
		public bool bookingConfirm { get; set; }
        public List<ConfirmedBookingTimeSlot> confirmedBookingTimeSlotList { get; set; }
	}

    public class Booking
    {
		public string bookingID { get; set; }
		public string invoiceNo { get; set; }
        public string facilityName { get; set; }
        public string facilityId { get; set; }
		public string unitNo { get; set; }
        public string unitID { get; set; }
		public string bookedBy { get; set; }
		public string period { get; set; }
        public string bookingDate { get; set; }
        public float bookingFee { get; set; }
		public float deposit { get; set; }
        public string status { get; set; }
		public string receiptNo { get; set; }
		public string photo { get; set; }
        public string _statusRemark { get; set; }
        public string _bookingDate { get; set; }
        public string blockNo { get; set; }
        public bool isSelected;
        public bool isCompleted;
    }


    public class CancelBookingRequest
    {
		public string remarks { get; set; }
        public string refundMethod { get; set; }
		public string chequeNo { get; set; }
		public string bank { get; set; }
    }

    public class ConfirmResponse : PaidBookingResponse
	{
        public string facilityName { get; set; }
        public string bookingDate { get; set; }
        public string userName { get; set; }
        public string timeSlotPeriod { get; set; }
        public string unitNo { get; set; }
        public List<AvailableBookingTimeSlot> confirmedBookingTimeSlotList { get; set; }
	}
}
