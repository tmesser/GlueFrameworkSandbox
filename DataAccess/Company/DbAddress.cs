namespace DataAccess.Company
{
    public class DbAddress
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip4 { get; set; }
        public string Zip5 { get; set; }
        public string GetFullZip()
        {
            return Zip5 + "-" + Zip4;
        }
    }
}
