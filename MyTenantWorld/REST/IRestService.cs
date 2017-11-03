
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyTenantWorld
{
	public interface IRestService
	{
		Task<LoginResponse> Login(Login user);
		Task<BaseResponse> ResetPassword(string email);
		Task<BaseResponse> ActivateAccount(ActivateInfo info);
		Task<HomeProfile> GetHomeProfile(bool fromlogin);
		Task<HomeProfile> GetHomeProfileWithPID(string pid);

		//01
		Task<List<Unit>> GetAllUnit(string pid);
		Task<List<Resident>> GetAllResident(string pid, string defaultUnitId);
		Task<SpecificResident> GetSpecificResident(string pid, string defaultUnitId, string defaultUserId);
		Task<SpecificResident> UpdateResident(string Pid, string defaultUnitId, string defaultUserId, SpecificResident request);
		Task<ResidentUser> InsertResident(string Pid, string defaultUnitId, string defaultUserId, ResidentUser request);
		Task<SpecificResident> InsertResident(string Pid, string defaultUnitId, SpecificResident request);
		Task<BaseResponse> DeleteResident(string Pid, string defaultUnitId, string defaultUserId);
		Task<List<SearchUnit>> SearchUserName(string Pid, string searchterm);

		//02
		Task<PortfolioInfo> GetPortfolioInfo(string Pid);
		Task<PortfolioInfo> PutPortfolioInfo(string Pid, PortfolioInfoRequest request);
		Task<Wordpress> GetWordpressInfo(string Pid);

		//03
		Task<BaseResponse> DeleteAgent(string defaultPid, Agent agent);
		Task<Agent> InsertAgent(string defaultPid, Agent agent);
		Task<List<Agent>> GetAgent(string defaultPid);
		Task<Agent> PutAgent(string defaultPid, Agent agent);

		//06
		Task<List<Staff>> GetAllStaff(string defaultPid);
		Task<SpecificStaff> GetSpecificStaff(string defaultPid, string staffId);
		Task<StaffResponse> InsertStaff(string defaultPid, SpecificStaff staff);
        Task<StaffResponse> UpdateStaff(string defaultPid, SpecificStaff staff, string staffId);
		Task<BaseResponse> DeleteStaff(string defaultPid, string staffId);

		//07
		Task<List<Committee>> GetCommittee(string defaultPid);
        Task<Dictionary<string, List<Committee>>> UpdateCommittee(string defaultPid, CommitteeRequest committees);

		//08
		Task<List<FacilityItem>> GetAllFacility(string defaultPid);
		Task<Facility> GetSpecificFacility(string defaultPid, string defaultFacilityID);
		Task<Facility> InsertFacility(string defaultPid, FacilityRequest facility);
		Task<Facility> GetNewlyInsertedFacility(string defaultPid, string facilityID);
		Task<Facility> UpdateFacility(string defaultPid, FacilityRequest facility, string facilityID);
		Task<Facility> DeleteFacility(string defaultPid, string facilityID);
		Task<Facility> GetFacilityGroup(string defaultPid);

		//12
		Task<BaseResponse> DeleteCustomization(string defaultPid);
		Task<Customization> InsertCustomization(string defaultPid,Customization customization);
		Task<Customization> GetCustomization(string defaultPid);
		Task<Customization> UpdateCustomization(string defaultPid,Customization customization);

		Task<List<Folder>> GetFolder(string defaultPid);
		Task<Folder> InsertFolder(string defaultPid, FolderRequest folder);
		Task<Folder> UpdateFolder(string defaultPid, string folderID, FolderRequest folder);
		Task<FileResponse> InsertFile(string defaultPid, string folderID, FileRequest file);
		Task<List<File>> GetFile(string defaultPid, string folderID);
		Task<File> UpdateFile(string defaultPid, string folderID, string fileID, FileRequest file);
		Task<BaseResponse> DeleteFile(string defaultPid, string folderID, string fileID);
		Task<BaseResponse> DeleteFolder(string defaultPid, string folderID);

		//Booking 443
        Task<List<string>> SelectBlockNo(string defaultPid);
		Task<List<BaseUnit>> SelectUnit(string defaultPid, string blockNo);
		Task<List<Tenant>> SelectResident(string defaultPid, string blockNo, string unitId);
		Task<FacilityBookingInfo> FacilityScreen(string defaultPid, string defaultFaciltyId, string unitId, string tenantId, string defaultBookingDate);
        Task<BookingDetails> FacilityBookingDetailsScreen(string defaultPid, string defaultFaciltyId, string unitId, string tenantId, string defaultBookingDate, BookingRequest request);
        Task<List<Tenant>> Reminder(string defaultPid, string unitId);
        Task<PaidBookingResponse> ConfirmBooking(string defaultPid, string defaultFaciltyId, string unitId, string tenantId, string defaultBookingDate, PaidBookingRequest request);
        Task<BaseResponse> AddNotifyResident(string defaultPid, string defaultFaciltyId, string unitId, string tenantId, string defaultBookingDate, NotifyResidentRequest request);
        Task<BookingDetails> DeleteNotifyResident(string defaultPid, string defaultFaciltyId, string unitId, string tenantId, string defaultBookingDate);
        //445
        Task<FacilityBookingInfo> FacilityScreen(string defaultPid, string defaultFaciltyId, string defaultBookingDate);
        Task<BookingResponse> ConfirmBooking(string defaultPid, string defaultFaciltyId, string defaultBookingDate, ReservedBookingRequest request);
        //453
        Task<List<Booking>> GetBookingList(string defaultPid, string status); 
        Task<List<Booking>> SearchBookingList(string defaultPid, string status, string searchTerm, string startDate, string endDate);
		Task<Receipt> GetReceipt(string defaultPID, string receiptID); 
        Task<BaseResponse> CancelBooking(string defaultPID, string defaultBookingID, CancelBookingRequest request);
        Task<ConfirmResponse> ConfirmPayment(string defaultPID, string defaultBookingID);
        Task<BaseResponse> ConfirmPayment(string defaultPID, string defaultBookingID, PaidBookingRequest request);
        Task<BaseResponse> CancelReservation(string defaultPID, string defaultBookingID);
        Task<ReceiptResponse> RefundDeposit(string defaultPID, string defaultBookingID);
        Task<BaseResponse> ForfeitDeposit(string defaultPID, string defaultBookingID,string reason);
        Task<TransactionHeader> GetTransactionHeader(string defaultPID);

        //504
        Task<List<Transaction>> ReceiptsRefundsScreen(string defaultPid);
        Task<List<Transaction>> SearchByTransactionNo(string defaultPid, string transaction);

        //Feedbacks

        Task<List<Feedback>> GetFeedback(string defaultPID, string status, string startDate="", string endDate="");
        Task<FeedbackDetail> GetFeedbackDetails(string defaultPID, string feedbackID);
        Task<UpdateFeedbackResponse> UpdateFeedback(string defaultPID, string feedbackID, UpdateFeedbackRequest request);
        Task<BaseResponse> LogOut(string defaultPID);

        //Adhocs
        Task<BaseResponse> CreateAdhocRefund(string defaultPID, string unitID, string tenantID, Refund refund);
        Task<List<Audit>> GetAuditListing(string defaultPID, string startDate=null, string endDate=null);

        //News
        Task<BaseResponse> InsertNews(string defaultPID, NewsRequest news);
        Task<List<News>> GetNews(string defaultPID);
        Task<News> GetNewsDetails(string defaultPID,string newsID);
        Task<BaseResponse> DeleteNews(string defaultPID, string newsID);
	} 
}
    