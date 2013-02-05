namespace Domain.Company
{
    public class DomainAddress
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public string Get5Zip()
        {
            return Zip.Substring(0,5);
        }

        public string Get4Zip()
        {
            return Zip.Substring(5,4);
        }
    }
}
