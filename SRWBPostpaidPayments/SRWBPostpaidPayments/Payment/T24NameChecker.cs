using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SRWBPostpaidPayments.Payment
{
    public class T24NameChecker
    {
        public string MakeNameT24Compliant(string name, int length = 14)
        {
            if (string.IsNullOrEmpty(name)) return name;

            if (name.Length <= length) return name;

            return name.Substring(0, length);
        
        }
    }
}