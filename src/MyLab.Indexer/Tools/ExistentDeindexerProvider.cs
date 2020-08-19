using System;
using System.Threading.Tasks;

namespace MyLab.Indexer.Tools
{
    class ExistentDeindexerProvider
    {
        private readonly string _indexName;
        private readonly IIndexManager _indexManager;

        public ExistentDeindexerProvider(string indexName, IIndexManager indexManager)
        {
            if (string.IsNullOrEmpty(indexName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(indexName));
            _indexName = indexName;
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));
        }

        public async Task<IDeindexer> ProvideAsync()
        {
            return await _indexManager.IsIndexExists(_indexName)
                ? await _indexManager.CreateDeindexerForExistent(_indexName)
                : await Task.FromResult<IDeindexer>(null);
        }
    }
}
