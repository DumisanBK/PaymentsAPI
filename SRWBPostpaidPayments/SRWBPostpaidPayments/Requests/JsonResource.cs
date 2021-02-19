using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SRWBPostpaidPayments.Requests
{
    public class JsonResource<T>
    {
        public int ErrorCode { get; set; }
        public T Data { get; set; }
    }
}