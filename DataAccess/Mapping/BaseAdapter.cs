using Glue;

namespace DataAccess.Mapping
{
    public class BaseAdapter<T, TS> : IObjectAdapter<T, TS>
    {
        private Mapping<T, TS> _mapper;

        public TS Adapt(T source, bool useAuto = false)
        {
            if (useAuto)
            {
                var autoMapping = new Mapping<T, TS>();
                autoMapping.AutoRelateEqualNames();

                return autoMapping.Map(source);
            }

            if (_mapper == null)
            {
                var mapping = new MappingFactory();
                _mapper = mapping.GetMapping<T, TS>();
            }
            
            var retValue = _mapper.Map(source);

            return retValue;
        }
    }
}
