using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SRWBPostpaidPayments.Requests
{
    public class PaymentRequest
    {
        public string TellerId { get; set; }
        public string TellerName { get; set; }
        public string TellerUsername { get; set; }
        public string TellerAccount { get; set; }
        public string BranchCode { get; set; }
        public string BranchMnemonic { get; set; }
        public string AccountNo { get; set; }
        public decimal Amount { get; set; }
        public string CustomerId { get; set; }
        public string GatePass { get; set; }
        public string AppPass { get; set; }
        public string AppName { get; set; }
        public string DecryptedPassword { get; set; }
        public string Branch { get; set; }
    }
}