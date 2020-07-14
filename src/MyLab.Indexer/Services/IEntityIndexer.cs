using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using MyLab.Elastic;
using MyLab.Logging;

namespace MyLab.Indexer.Services
{
    interface IEntityIndexer
    {
        Task IndexEntityBatchAsync(DbEntity[] docBatch);

        Task RemoveEntitiesAsync(string[] ids);

        Task StartReindexAsync();

        Task EndReindexAsync();
    }

    class DefaultEntityIndexer : IEntityIndexer
    {
        private readonly IEsManager _esManager;
        private readonly IndexerOptions _options;

        public DefaultEntityIndexer(IEsManager esManager, IOptions<IndexerOptions> options)
        {
            _esManager = esManager;
            _options = options.Value;
        }

        public async Task IndexEntityBatchAsync(DbEntity[] docBatch)
        {
            var exRes = await _esManager.Client.Indices.ExistsAsync(_options.Index);
            exRes.ThrowIfInvalid("Can't check index for existing");

            var realIndex = GetRealIndexName();

            if (!exRes.Exists)
            {
                var crIndexResp = await _esManager.Client.Indices.CreateAsync(realIndex);
                crIndexResp.ThrowIfInvalid("Can't create index");

                var crAliasResp = await _esManager.Client.Indices.PutAliasAsync(realIndex, _options.Index);
                crAliasResp.ThrowIfInvalid("Can't create alias for index");
            }

            var indexer = new DbEntityIndexer(_options.Index, _esManager.Client.LowLevel);

            var indexResp = await indexer.Index(docBatch);
            indexResp.ThrowIfInvalid("Can't index entities");
        }

        public Task RemoveEntitiesAsync(string[] ids)
        {
            throw new System.NotImplementedException();
        }

        public Task StartReindexAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task EndReindexAsync()
        {
            throw new System.NotImplementedException();
        }

        string GetRealIndexName() => _options.Index + "-real";
    }

    interface IIndexMappingProvider
    {

    }
}