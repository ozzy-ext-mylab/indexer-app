using System.Collections.Generic;

namespace MyLab.Indexer.Tools
{
    interface IOriginEntityProvider 
    {
        IAsyncEnumerable<OriginEntity[]> Provide(IOriginEntitySource src, int pageSize);
    }
}
