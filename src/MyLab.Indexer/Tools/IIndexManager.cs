using System.Threading.Tasks;

namespace MyLab.Indexer.Tools
{
    public interface IIndexManager
    {
        Task<IIndexer> CreateIndexerForExistentAsync(string indexName);

        Task<IDeindexer> CreateDeindexerForExistentAsync(string indexName);

        Task<IIndexer> CreateIndexAsync(string indexName);

        Task RemoveIndexAsync(string indexName);

        Task AliasIndex(string alias, string indexName);

        Task<bool> IsIndexExistsAsync(string indexName);

        Task<string[]> GetAliasIndices(string aliasName);
    }

    public interface IDeindexer
    {
        Task DeindexAsync(string[] ids);
    }
}