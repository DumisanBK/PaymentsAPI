using LibOFS_R14.OFSRequest;
using LibOFS_R14.OFSResponse;
using SRWBPostpaidPayments.Data;
using SRWBPostpaidPayments.Repository;
using SRWBPostpaidPayments.Requests;
using SRWBPostpaidPayments.SRWBService;
using SRWBPostpaidPayments.T24Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SRWBPostpaidPayments.Payment
{
    
    public class OFSBuilder
    {
        private readonly SRWBPostpaidContext _postpaidContext;
        private readonly PaymentRequest _paymentRequest;
        private readonly T24NameChecker _t24NameChecker;
        private readonly SRWBPaymentsAPIServiceQueryAccountResponse _customer;
        public OFSBuilder(SRWBPostpaidContext postpaidContext, PaymentRequest paymentRequest, T24NameChecker t24NameChecker, SRWBPaymentsAPIServiceQueryAccountResponse customer)
        {

            _postpaidContext = postpaidContext;
            _paymentRequest = paymentRequest;
            _t24NameChecker = t24NameChecker;
            _customer = customer;
        }

        public async Task<OFSResult> MakeRequest() 
        {
            var ofsResult = new OFSResult();
            var operations = new OperationOptions(FunctionEnum.Input, ProcessingFlagEnum.Process, GTSControlEnum.UseDefault, AuthorisersEnum.ForceNoAuthorization);

            var userInfo = new UserInfo
            {
                CompanyID = _paymentRequest.BranchCode,
                Username = _paymentRequest.TellerUsername,
                Password = _paymentRequest.DecryptedPassword
            };

            var ofsRequest = new TransactionRequest(WebConfigValues.FtName(), ref operations, userInfo);

            var transcationFields = new TransactionDataFields();

            transcationFields.Fields = new List<TransactionDataField>
            {

                new TransactionDataField("DEBIT.CURRENCY", "MWK"),
                new TransactionDataField("WB.UTILITY.ACCT", _t24NameChecker.MakeNameT24Compliant(_paymentRequest.AccountNo)),
                new TransactionDataField("WB.CUST.NAME", _t24NameChecker.MakeNameT24Compliant(_customer.response.parmCustomerName)),
                new TransactionDataField("DEBIT.ACCT.NO", _paymentRequest.TellerAccount, 1, 1),
                new TransactionDataField("DEBIT.THEIR.REF", _t24NameChecker.MakeNameT24Compliant(_paymentRequest.AccountNo)),
                new TransactionDataField("DEBIT.AMOUNT", _paymentRequest.Amount.ToString("#############0.00")),
                new TransactionDataField("CREDIT.CURRENCY", "MWK"),
                new TransactionDataField("CREDIT.THEIR.REF", _t24NameChecker.MakeNameT24Compliant(_paymentRequest.AccountNo)),
                new TransactionDataField("PROFIT.CENTRE.DEPT", Convert.ToString(9999)),
                new TransactionDataField("PAYMENT.DETAILS", WebConfigValues.PaymentDetails())
             };

            var ofsMessage = ofsRequest.ToOFSString(transcationFields, null, $"{WebConfigValues.FtPrefix()}{Regex.Replace(Guid.NewGuid().ToString(), "[^0-9a-z]*", "")}");

            try
            {
                OFSTxn response = await new T24ClientSoapClient().PostRawOFSAsync(ofsMessage);

                if (response.SocketResponse)
                {
                    if (response.T24Response)
                    {
                        ofsResult.Success = true;
                        ofsResult.Message = "The transaction was successful";
                        ofsResult.TransactionId = await GetT24Ft(response.OFSResponse, response.TxnRef);
                        ofsResult.IsDuplicateTrapResponse = response.IsDuplicateTrapResponse;
                    }
                    else if (response.IsDuplicateTrapResponse)
                    {
                        ofsResult.Success = true;
                        ofsResult.Message = "The transaction was successful though it is a duplicate trap response";
                        ofsResult.TransactionId = await GetT24Ft(response.OFSResponse, response.TxnRef);
                        ofsResult.IsDuplicateTrapResponse = true;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(response.PossibleErrorMessage) && !string.IsNullOrEmpty(response.OFSResponse))
                        {
                            if (response.OFSResponse.Contains("OFSERROR_TIMEOUT"))
                            {
                                ofsResult.Message = "A timeout error occured at T24 level";
                            }
                            else if (response.OFSResponse.Contains("SECURITY VIOLATION"))
                            {
                                ofsResult.Message = "Incorrect credentials";
                            }
                            else if (response.OFSResponse.Contains("NO SIGN ON NAME SUPPLIED"))
                            {
                                ofsResult.Message = "Login credentials were not supplied";
                            }
                            else
                            {
                                ofsResult.Message = "An error occured";
                            }
                        }
                    }
                }
                else
                {
                    ofsResult.Message = "Could not establish a connection with T24 server";
                }

                ofsResult.Request = response.OFSRequest.Replace(userInfo.Password, "******");
                ofsResult.Response = response.OFSResponse ?? string.Empty;
            }
            catch (Exception ex)
            {
                ofsResult.Message = ex.Message ?? "An error occurred!";

                await new ExceptionRepository(_postpaidContext).SaveAsync(
                    ofsResult.Message, nameof(OFSBuilder), nameof(MakeRequest), _paymentRequest.TellerUsername);
            }

            return ofsResult;
        }

        private async Task<string> GetT24Ft(string ofsResponse, string passedFt)
        {
            string ft = string.Empty;
            try
            {
                var transactionResponse = new R14TransactionResponse(ofsResponse);

                var possibleFtNames = new string[] { "DEBIT.ACCT.NO", "DEBIT.CURRENCY", "DEBIT.VALUE.DATE", "TRANSACTION.TYPE" };

                var firstResponse = transactionResponse.FindTransactionsByFieldNames(possibleFtNames).First();

                ft = firstResponse.RecordID;

            }
            catch (Exception ex)
            {
                string innerMessage = ex.Message ?? "Error while fetching T24 FT";

                await new ExceptionRepository(_postpaidContext).SaveAsync(
                    innerMessage, nameof(OFSBuilder), nameof(GetT24Ft), _paymentRequest.TellerUsername);
            }

            finally
            {
                if (string.IsNullOrEmpty(ft)) ft = passedFt;
            }

            return ft;
        }
    }
}