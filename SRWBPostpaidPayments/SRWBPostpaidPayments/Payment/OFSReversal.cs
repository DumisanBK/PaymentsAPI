using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SRWBPostpaidPayments.Payment
{
    public class OFSReversal
    {
        public async Task<bool> Reverse(string transactionRef, string companyCode, int port)
        {
            bool reversed;
            try
            {
                var reversedData = await new T24Service.T24ClientSoapClient().ReverseTransactionAsync(transactionRef,companyCode,port);
                reversed = reversedData.T24Response;
            }
            catch (Exception)
            {

                reversed = false;
            }
            return reversed;
        }
    }
}