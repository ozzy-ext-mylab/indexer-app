using System.Threading.Tasks;

namespace MyLab.Indexer.Tools
{
    public interface IIndexManager
    {
        Task<IIndexer> CreateIndexerForExistent(string indexName);

        Task<IIndexer> CreateIndex(string indexName);
        Task CreateAlias(string alias, string indexName);

        Task<bool> IsIndexExists(string indexName);
    }
}