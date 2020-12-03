using System.Threading.Tasks;

namespace MyLab.Indexer.Common
{
    public interface IIndexerProvider
    {
        Task<IIndexer> ProvideIndexerAsync();
    }
}