using SRWBPostpaidPayments.Data;
using SRWBPostpaidPayments.Payment;
using SRWBPostpaidPayments.Requests;
using SRWBPostpaidPayments.SRWBService;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SRWBPostpaidPayments.Repository
{
    public class TransactionsRepository
    {
        private readonly SRWBPostpaidContext _postpaidContext;
        public TransactionsRepository(SRWBPostpaidContext postpaidContext)
        {
            _postpaidContext = postpaidContext;
        }

        public async Task<Transaction> SaveAsync(PaymentRequest paymentRequest, OFSResult oFSResult, SRWBPaymentsAPIServiceQueryAccountResponse customer )
        {
            Transaction transaction = null;

            EventLog eventLogger = new EventLog
            {
                Source = "SRWBPOSTPAID"
            };
           
            try
            {
                transaction = MakeTransaction(paymentRequest, customer, oFSResult, oFSResult.Success);
                _postpaidContext.Transactions.Add(transaction);
                await _postpaidContext.SaveChangesAsync();

            }
            catch (DbEntityValidationException ex)
            {

                foreach (var eve in ex.EntityValidationErrors)
                {
                    eventLogger.WriteEntry(eve.Entry.Entity.GetType().Name + "" + eve.Entry.State, EventLogEntryType.Error);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        eventLogger.WriteEntry(ve.PropertyName + "" + ve.ErrorMessage, EventLogEntryType.Error);
                    }
                }

                throw;
            }

            return transaction;
        }
        public Transaction MakeTransaction(PaymentRequest paymentResource, SRWBPaymentsAPIServiceQueryAccountResponse customer, OFSResult ofsResult, bool success)
        {
            
            var txnId = string.IsNullOrEmpty(ofsResult.TransactionId) ? "0" : ofsResult.TransactionId;

            var transaction = new Transaction
            {
                AccessingPhone = null,
                Amount = Convert.ToDecimal(paymentResource.Amount),               
                EffectivePhone = null,
                StatusCode = success? 000000065 : 0,
                StatusDescription = success ? "Successfully submitted the payment with reference" : "Payment not made",
                FriendlyName = customer.response.parmCustomerName,
                FundingAcct = paymentResource.TellerAccount,
                Message = success ? "Payment made" : "Payment failed",
                CustomerName = customer.response.parmCustomerName,
                OfsRequest = ofsResult.Request,
                OfsResponse = ofsResult.Response,
                ProfileID = null,
                PaymentReference = txnId,
                Reversed = false,
                PaymentDate = DateTime.Now,
                PaymentMode = "CASH",
                TxnRef = txnId,
                Teller = paymentResource.TellerName ?? "N/A",
                Balance = 0,
                Currency = "MWK",
                PaymentId = txnId,
                CustomerNumber = paymentResource.AccountNo
            };

            return transaction;
        }
    }
}