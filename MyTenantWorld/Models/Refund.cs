using System.Collections.Generic;
using System.Net;

namespace MyTenantWorld
{
	public class Refund
    {
        public string paymentMethod { get; set; }
        public double totalPayment { get; set; }
        public string chequeNo { get; set; }
        public string bank { get; set; }
        public string transactionType { get; set; }
        public List<ReceiptDetail> adhocTransactionDetail { get; set; }
    }
}
