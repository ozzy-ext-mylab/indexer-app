using System.Threading.Tasks;

namespace MyLab.Indexer.Tools
{
    public interface IIndexer
    {
        Task IndexAsync(OriginEntity[] entities);
    }
}