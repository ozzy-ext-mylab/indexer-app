using System;
using System.Threading.Tasks;

namespace MyLab.Indexer.Tools
{
    class ReindexOperator : IAsyncDisposable
    {
        public string IndexName { get; }
        private readonly IIndexManager _indexManager;

        ReindexOperator(string aliasName, string indexName, IIndexManager indexManager)
        {
            if (string.IsNullOrEmpty(aliasName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(aliasName));
            IndexName = indexName;
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));
        }

        public static async Task<ReindexOperator> StartReindexingAsync(string aliasName, IIndexManager indexManager)
        {
            if (indexManager == null) throw new ArgumentNullException(nameof(indexManager));
            if (string.IsNullOrEmpty(aliasName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(aliasName));

            var indexName = NewIndexNameBuilder.Build(aliasName);

            await indexManager.CreateIndex(indexName);

            return new ReindexOperator(aliasName, indexName, indexManager);
        }

        public async Task<IIndexer> GetIndexerAsync()
        {
            return await _indexManager.CreateIndexerForExistent(IndexName);
        }

        //public Task CommitAsync()
        //{

        //}

        public async ValueTask DisposeAsync()
        {
            await _indexManager.RemoveIndex(IndexName);
        }
    }
}
