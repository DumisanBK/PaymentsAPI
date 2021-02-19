using SRWBPostpaidPayments.Data;
using SRWBPostpaidPayments.Payment;
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
using System.Web.Http.Description;

namespace SRWBPostpaidPayments.Controllers
{
    [RoutePrefix("api/customer")]
    public class CustomersController : ApiController
    {
        // GET: api/accountNo/ZZA04971
        [HttpGet]
        [ResponseType(typeof(JsonResource<Customer>))]
        [Route("accountNo")]
        public async Task<IHttpActionResult> GetCustomer(string accountNo)
        {
            SRWBPaymentsAPIServiceClient _serviceClient = new SRWBPaymentsAPIServiceClient();

            _serviceClient.ClientCredentials.Windows.ClientCredential.UserName = WebConfigValues.Username();
            _serviceClient.ClientCredentials.Windows.ClientCredential.Password = WebConfigValues.Password();

            SRWBPaymentsAPIServiceQueryAccountRequest inValue = new SRWBPaymentsAPIServiceQueryAccountRequest
            {
                accountNumber = accountNo
            };

            var result = new JsonResource<SRWBPaymentsAPIServiceQueryAccountResponse>();

            var customer = await _serviceClient.queryAccountAsync(inValue.accountNumber);

            result.ErrorCode = customer == null ? 1 : 0;

            result.Data = customer;

            return Ok(result);
        }

    }
}
