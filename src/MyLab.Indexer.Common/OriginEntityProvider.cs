using System.Collections.Generic;

namespace MyLab.Indexer.Common
{
    public class OriginEntityProvider
    {
        private readonly IOriginEntityProviderLogic _logic;
        private readonly IOriginEntitySource _src;
        private readonly int _pageSize;

        public OriginEntityProvider(IOriginEntityProviderLogic logic, IOriginEntitySource src, int pageSize)
        {
            _logic = logic;
            _src = src;
            _pageSize = pageSize;
        }

        public IAsyncEnumerable<OriginEntity[]> Provide()
        {
            return _logic.Provide(_src, _pageSize);
        }
    }
}