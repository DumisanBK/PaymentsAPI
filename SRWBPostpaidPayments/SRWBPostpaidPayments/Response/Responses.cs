using SRWBPostpaidPayments.SRWBService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SRWBPostpaidPayments
{
    public class Responses
    {
        public SRWBPaymentsAPIServiceQueryAccountResponse AccountResponse { get; set; }

        public SRWBPaymentsAPIServicePostPaymentResponse PostPaymentResponse { get; set; }

        public SRWBPaymentsAPIServiceReversePaymentResponse ReversePaymentResponse { get; set; }

        public SRWBPaymentsAPIServiceTransferPaymentResponse TransferPaymentResponse { get; set; }
    }
}