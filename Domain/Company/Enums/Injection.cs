namespace Domain.Company.Enums
{

    /// <summary>
    /// Allows for cascade injection through structuremap (or whatever IoC container makes you happiest really)
    /// </summary>
    public enum Injection
    {
        ServiceLayer,
        DataAccess
    }
}
