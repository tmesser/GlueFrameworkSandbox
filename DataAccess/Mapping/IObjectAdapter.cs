namespace DataAccess.Mapping
{
    public interface IObjectAdapter<in TSource, out TDestination>
    {
        TDestination Adapt(TSource source, bool useAuto = false);
    }
}
