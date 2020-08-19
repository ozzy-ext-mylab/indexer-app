using System;
using System.Threading.Tasks;

namespace MyLab.Indexer.Tools
{
    class ReindexOperator : IAsyncDisposable, IIndexerProvider
    {
        private readonly IIndexManager _indexManager;
        private bool _commited;

        public string IndexName { get; }
        private string AliasName { get; }
        
        ReindexOperator(string aliasName, string indexName, IIndexManager indexManager)
        {
            if (string.IsNullOrEmpty(aliasName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(aliasName));
            IndexName = indexName;
            AliasName = aliasName;
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));
        }

        public static async Task<ReindexOperator> StartReindexingAsync(string aliasName, IIndexManager indexManager)
        {
            if (indexManager == null) throw new ArgumentNullException(nameof(indexManager));
            if (string.IsNullOrEmpty(aliasName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(aliasName));

            var indexName = NewIndexNameBuilder.Build(aliasName);

            await indexManager.CreateIndexAsync(indexName);

            return new ReindexOperator(aliasName, indexName, indexManager);
        }

        public async Task<IIndexer> ProvideIndexerAsync()
        {
            return await _indexManager.CreateIndexerForExistentAsync(IndexName);
        }

        public async Task CommitAsync()
        {
            var isAliasExists = await _indexManager.IsIndexExistsAsync(AliasName);
            string[] oldIndices = null;

            if (isAliasExists)
                oldIndices = await _indexManager.GetAliasIndices(AliasName);

            await _indexManager.AliasIndex(AliasName, IndexName);

            if (oldIndices != null)
            {
                foreach (var oldIndex in oldIndices)
                {
                    await _indexManager.RemoveIndexAsync(oldIndex);
                }
            }

            _commited = true;
        }

        public async ValueTask DisposeAsync()
        {
            if(!_commited)
                await _indexManager.RemoveIndexAsync(IndexName);
        }
    }
}
