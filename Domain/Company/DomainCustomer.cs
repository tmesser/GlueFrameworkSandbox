using System;

namespace Domain.Company
{
    public class DomainCustomer
    {
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Id { get; set; }
        public string[] AccountNumbers { get; set; }
        public DateTime DateJoined { get; set; }
    }
}
