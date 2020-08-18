using System.Linq;

namespace MyLab.Indexer.Tools
{
    interface IOriginEntitySource
    {
        IQueryable<OriginEntity> Query();
    }
}