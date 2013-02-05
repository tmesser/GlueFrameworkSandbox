namespace DataAccess.Company
{
    public class ResultDetails
    {
        public bool Success { get; set; }
        public WarningInfo[] Warnings { get; set; }
        public ErrorInfo[] Errors { get; set; }
    }
}
