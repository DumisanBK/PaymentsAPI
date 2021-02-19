using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SRWBPostpaidPayments.Payment
{
    public class OFSResult
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public bool IsDuplicateTrapResponse { get; set; } = false;
    }
}