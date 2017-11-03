using System.Collections.Generic;
using System.Net;

namespace MyTenantWorld
{
    public class TransactionHeader
    {
        public string portfolioLogo { get; set; }
        public string portfolioName { get; set; }
        public string website { get; set; }
        public string strataTitle { get; set; }
        public string agencyName { get; set; }
        public string agencyAddress { get; set; }
        public string agencyMobile { get; set; }
        public string agencyFax { get; set; }
        public string staffName { get; set; }
        public string receiptNo { get; set; }
    }
	public class Receipt: TransactionHeader
    {
		public string id { get; set; }
		public string portfolioLogo { get; set; }
		
        public string familyName { get; set; }
        public string givenName { get; set; }
        public string unitNo { get; set; }  

        public string gstRegNo { get; set; }

        public string date { get; set; }
        public string paymentMethod { get; set; }
        public double totalPayment { get; set; }
        public string chequeNo { get; set; }
        public string bank { get; set; }
        public string transactionType { get; set; }
        public string facilityImage { get; set; }
        public List<ReceiptDetail> receiptDetailList { get; set; }
    }
    public class ReceiptResponse : Receipt
	{
        public HttpStatusCode status_code { get; set; }
        public string message { get; set; }
	}

	public class Transaction 
    {
        public string transactionNo { get; set; }
        public float transactionAmount { get; set; }
        public string resident { get; set; }
        public string type { get; set; }
        public string transactionType { get; set; }
	    public string _transactionDate { get; set; }
	    public string bookingID { get; set; }
	    public string _actionType { get; set; }
	    public string blockNo { get; set; }
    }

    public class ReceiptDetail
    {
        public string refNo { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public float amount { get; set; }
        public string remarks { get; set; }
    }

}
