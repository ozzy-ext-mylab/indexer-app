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
            if (await _indexManager.IsIndexExists(_indexName))
                return await _indexManager.CreateIndexerForExistent(_indexName);

            var evenIndexName = GetEvenIndex(_indexName);
            
            if (!await _indexManager.IsIndexExists(evenIndexName))
                await _indexManager.CreateIndex(evenIndexName);
            await _indexManager.CreateAlias(_indexName, evenIndexName);

            return await _indexManager.CreateIndexerForExistent(_indexName);
        }

        public static string GetEvenIndex(string baseIndexName) => baseIndexName + "-real-even";
    }
}
