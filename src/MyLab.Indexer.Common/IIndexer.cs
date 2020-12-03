using System.Threading.Tasks;

namespace MyLab.Indexer.Common
{
    public interface IIndexer
    {
        Task IndexAsync(OriginEntity[] entities);
    }
}