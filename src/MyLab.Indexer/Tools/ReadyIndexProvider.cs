using System;
using System.Threading.Tasks;

namespace MyLab.Indexer.Tools
{
    class ReadyIndexProvider
    {
        private readonly IIndexManager _indexManager;
        private readonly string _indexName;

        public ReadyIndexProvider(IIndexManager indexManager, string indexName)
        {
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));
            _indexName = indexName;
        }

        public async Task<IIndexer> Provide()
        {
            var evenIndexName = GetEvenIndex(_indexName);
            if (await _indexManager.IsIndexExists(evenIndexName))
                return await _indexManager.CreateIndexerForExistent(evenIndexName);

            var oddIndexName = GetOddIndex(_indexName);
            if (await _indexManager.IsIndexExists(oddIndexName))
                return await _indexManager.CreateIndexerForExistent(oddIndexName);

            var indexer = await _indexManager.CreateIndex(evenIndexName);
            await _indexManager.CreateAlias(_indexName, evenIndexName);

            return indexer;
        }

        string GetEvenIndex(string baseIndexName) => baseIndexName + "-real-even";
        string GetOddIndex(string baseIndexName) => baseIndexName + "-real-odd";
    }

    interface IIndexer
    {
        Task Index(OriginEntity[] entities);
    }

    interface IIndexManager
    {
        Task<IIndexer> CreateIndexerForExistent(string indexName);

        Task<IIndexer> CreateIndex(string indexName);
        Task CreateAlias(string alias, string indexName);

        Task<bool> IsIndexExists(string indexName);
    }
}
