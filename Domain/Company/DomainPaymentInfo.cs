using System;
using System.Collections.Generic;

namespace Domain.Company
{
    public class DomainPaymentInfo
    {
        public IEnumerable<DateTime> SpecialPaymentDates { get; set; }
        public int SerialNumber { get; set; }
        public DateTime NextPaymentDate { get; set; }
        public string SourceAccount { get; set; }
        public Enums.Currency CurrencyType { get; set; }
    }
}
