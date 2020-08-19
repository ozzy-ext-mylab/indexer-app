using System.Threading.Tasks;

namespace MyLab.Indexer.Tools
{
    interface IIndexerProvider
    {
        Task<IIndexer> ProvideIndexerAsync();
    }
}