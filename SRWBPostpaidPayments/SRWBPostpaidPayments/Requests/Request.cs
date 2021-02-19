using SRWBPostpaidPayments.SRWBService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SRWBPostpaidPayments.Requests
{
    public class Request
    {
        public SRWBPaymentsAPIServiceQueryAccountRequest AccountRequest { get; set; }
        public SRWBPaymentsAPIServicePostPaymentRequest  PostPaymentRequest { get; set; }
        public SRWBPaymentsAPIServiceReversePaymentRequest ReversePaymentRequest { get; set; }
        public SRWBPaymentsAPIServiceTransferPaymentRequest TransferPaymentRequest { get; set; }
    }
}