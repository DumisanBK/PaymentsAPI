using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SRWBPostpaidPayments.Requests
{
    public class RawPaymentRequest
    {
        public string PaymentReference { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } 
        public string PaymentMode { get; set; }
    }
}