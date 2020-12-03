using System.Collections.Generic;

namespace MyLab.Indexer.Common
{
    public interface IOriginEntityProviderLogic 
    {
        IAsyncEnumerable<OriginEntity[]> Provide(IOriginEntitySource src, int pageSize);
    }
}
