using System.Threading.Tasks;

namespace MyLab.Indexer.Tools
{
    public interface IIndexer
    {
        Task Index(OriginEntity[] entities);
    }
}