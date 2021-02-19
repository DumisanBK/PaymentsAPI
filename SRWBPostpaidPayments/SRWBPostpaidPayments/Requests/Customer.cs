using SRWBPostpaidPayments.SRWBService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SRWBPostpaidPayments.Requests
{
    public class Customer
    {
        public string CustomerName { get; set; }
        public decimal AmountOutStanding { get; set; }
    }
}