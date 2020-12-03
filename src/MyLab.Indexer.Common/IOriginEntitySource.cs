using System.Linq;

namespace MyLab.Indexer.Common
{
    public interface IOriginEntitySource
    {
        IQueryable<OriginEntity> Query();
    }
}