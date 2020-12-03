using System.Threading.Tasks;

namespace MyLab.Indexer.Common
{
    public class EntityIndexer
    {
        private readonly IIndexerProvider _indexerProvider;
        private readonly OriginEntityProvider _entityProvider;

        public EntityIndexer(IIndexerProvider indexerProvider, OriginEntityProvider entityProvider)
        {
            _indexerProvider = indexerProvider;
            _entityProvider = entityProvider;
        }

        public async Task IndexAsync()
        {
            var indexer = await _indexerProvider.ProvideIndexerAsync();
            await foreach (var entityPage in _entityProvider.Provide())
            {
                await indexer.IndexAsync(entityPage);
            }
        }
    }
}