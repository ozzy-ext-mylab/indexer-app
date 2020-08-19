using System.Collections.Generic;

namespace MyLab.Indexer.Tools
{
    interface IOriginEntityProviderLogic 
    {
        IAsyncEnumerable<OriginEntity[]> Provide(IOriginEntitySource src, int pageSize);
    }
}
