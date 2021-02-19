using SRWBPostpaidPayments.Data;
using SRWBPostpaidPayments.Payment;
using SRWBPostpaidPayments.Repository;
using SRWBPostpaidPayments.Requests;
using SRWBPostpaidPayments.SRWBService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SRWBPostpaidPayments.Controllers
{
    [RoutePrefix("api/srwbpostpaid")]
    public class PaymentsController : ApiController
    {

        private readonly SRWBPaymentsAPIServiceClient _serviceClient = new SRWBPaymentsAPIServiceClient();
        private readonly SRWBPostpaidContext _context = new SRWBPostpaidContext();
        private readonly T24NameChecker _t24NameChecker = new T24NameChecker();


        // GET: api/Payments
        [HttpPost]
        [Route("postpayment")]
        public async Task<IHttpActionResult> MakePayment(PaymentRequest paymentResource)
        {
            EventLog eventLog = new EventLog
            {
                Source = "SRWBPOSTPAID"
            };

            eventLog.WriteEntry("Payment request" + "" + paymentResource.AccountNo + "" + paymentResource.Amount + ""+ paymentResource.AppName + "" +
                paymentResource.AppPass + ""+ paymentResource.Branch + "" + paymentResource.BranchCode + "" + paymentResource.BranchMnemonic + "" +
                paymentResource.CustomerId + "" + paymentResource.DecryptedPassword + "" + paymentResource.GatePass + "" + paymentResource.TellerAccount + "" +
                paymentResource.TellerId + "" + paymentResource.TellerName + "" + paymentResource.TellerUsername,EventLogEntryType.Error);

            var transaction = new Transaction();
            if (paymentResource == null)
            {
                return Ok(transaction);

            }

            var result = new JsonResource<Transaction>();

            string accountNo = paymentResource.CustomerId;

            SRWBPaymentsAPIServiceClient _serviceClient = new SRWBPaymentsAPIServiceClient();

            _serviceClient.ClientCredentials.Windows.ClientCredential.UserName = WebConfigValues.Username();
            _serviceClient.ClientCredentials.Windows.ClientCredential.Password = WebConfigValues.Password();

            SRWBPaymentsAPIServiceQueryAccountRequest inValue = new SRWBPaymentsAPIServiceQueryAccountRequest
            {
                accountNumber = accountNo
            };

            var customer = await _serviceClient.queryAccountAsync(inValue.accountNumber);
            EventLog eventLogger = new EventLog
            {
                Source = "SRWBPOSTPAID"
            };

            try
            {
                var ofsResult = await new OFSBuilder(_context, paymentResource, _t24NameChecker,customer).MakeRequest();

                transaction = await new TransactionsRepository(_context).SaveAsync(paymentResource, ofsResult, customer);

                await _serviceClient.postPaymentAsync(transaction.TxnRef, transaction.CustomerNumber, Convert.ToDecimal(transaction.Amount),transaction.PaymentDate, transaction.PaymentMode);
                eventLogger.WriteEntry(transaction.TxnRef + "" + transaction.CustomerNumber + "" + transaction.Amount + "" + transaction.PaymentDate, EventLogEntryType.Error);

                RemoveSensitiveDataInTransaction(ref transaction);
            }
            catch (Exception ex)
            {

                await new ExceptionRepository(_context).SaveAsync(
                    ex.ToString(), nameof(PaymentsController), nameof(MakePayment), paymentResource.TellerUsername ?? "N/A");
            }

            result.ErrorCode = transaction == null ? 1 : 0;
            result.Data = transaction;

            return Ok(result);
        }

        [HttpPost]
        [Route("srwbpostpayment")]
        public async Task<IHttpActionResult> PostPaymentAsync(SRWBRequest paymentRequest)
        {
          
            SRWBPaymentsAPIServiceClient _serviceClient = new SRWBPaymentsAPIServiceClient();

            _serviceClient.ClientCredentials.Windows.ClientCredential.UserName = WebConfigValues.Username();
            _serviceClient.ClientCredentials.Windows.ClientCredential.Password = WebConfigValues.Password();

            SRWBPaymentsAPIServicePostPaymentRequest postPaymentRequest = new SRWBPaymentsAPIServicePostPaymentRequest
            {
                paymentReference = paymentRequest.PaymentReference,
                accountNumber = paymentRequest.AccountNumber,
                amount = paymentRequest.Amount,
                paymentDate = paymentRequest.PaymentDate,
                paymentMode = paymentRequest.PaymentMode

            };

           var result = await _serviceClient.postPaymentAsync(postPaymentRequest.paymentReference, postPaymentRequest.accountNumber, postPaymentRequest.amount, postPaymentRequest.paymentDate, postPaymentRequest.paymentMode);
            return Ok(result.response.parmPaymentID);
        }

        private void RemoveSensitiveDataInTransaction(ref Transaction transaction)
        {
            transaction.OfsRequest = string.Empty;
            transaction.OfsResponse = string.Empty;
            transaction.FundingAcct = string.Empty;
            transaction.CustomerName = _t24NameChecker.MakeNameT24Compliant(transaction.CustomerName);
        }
    }
}
