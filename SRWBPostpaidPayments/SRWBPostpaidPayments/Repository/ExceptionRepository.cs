using SRWBPostpaidPayments.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SRWBPostpaidPayments.Repository
{
    public class ExceptionRepository
    {
        private readonly SRWBPostpaidContext _paymentsContext;
        public ExceptionRepository(SRWBPostpaidContext paymentsContext)
        {
            _paymentsContext = paymentsContext;
        }
        public async Task SaveAsync(string innerMessage, string className, string method, string user)
        {

            var exception = new PostPaidException 
            {
                Class = className,
                DateGenerated = DateTime.Now,
                InnerMessage = innerMessage,
                Method = method,
                SignedInUser = user
            };
            try
            {

                _paymentsContext.PostPaidExceptions.Add(exception);
                await _paymentsContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.ToString());
            }
        }
    }
}