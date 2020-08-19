using System;
using System.Threading.Tasks;

namespace MyLab.Indexer.Tools
{
    class ReadyIndexProvider
    {
        private readonly IIndexManager _indexManager;
        private readonly string _aliasName;

        public ReadyIndexProvider(IIndexManager indexManager, string aliasName)
        {
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));
            _aliasName = aliasName;
        }

        public async Task<IIndexer> ProvideAsync()
        {
            if (await _indexManager.IsIndexExists(_aliasName))
                return await _indexManager.CreateIndexerForExistent(_aliasName);

            var indexName = NewIndexNameBuilder.Build(_aliasName);
            
            await _indexManager.CreateIndex(indexName);
            await _indexManager.CreateAlias(_aliasName, indexName);

            return await _indexManager.CreateIndexerForExistent(_aliasName);
        }
    }
}
