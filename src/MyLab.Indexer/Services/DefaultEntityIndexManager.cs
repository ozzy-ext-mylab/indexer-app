using System;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using MyLab.Elastic;
using MyLab.Logging;

namespace MyLab.Indexer.Services
{
    class DefaultEntityIndexManager : IEntityIndexManager
    {
        private readonly IEsManager _esManager;
        private readonly IndexOptions _indexOptions;

        public DefaultEntityIndexManager(IEsManager esManager, IOptions<IndexerOptions> options)
            :this(esManager, options.Value.Index)
        {
        }

        public DefaultEntityIndexManager(IEsManager esManager, IndexOptions indexOptions)
        {
            _esManager = esManager;
            _indexOptions = indexOptions;
        }

        public async Task IndexEntityBatchAsync(DbEntity[] docBatch)
        {
            var exRes = await _esManager.Client.Indices.ExistsAsync(_indexOptions.IndexName);
            //exRes.ThrowIfInvalid("Can't check index for existing");

            if (!exRes.Exists)
            {
                var realIndex = GetRealIndexName(_indexOptions.IndexName);

                var crIndexResp = await _esManager.Client.Indices.CreateAsync(realIndex);
                crIndexResp.ThrowIfInvalid("Can't create index");

                var crAliasResp = await _esManager.Client.Indices.PutAliasAsync(realIndex, _indexOptions.IndexName);
                crAliasResp.ThrowIfInvalid("Can't create alias for index");
            }

            var indexer = new DbEntityIndexer(_indexOptions, _esManager.Client.LowLevel);

            var indexResp = await indexer.Index(docBatch);
            CheckIndexResponse(indexResp);
        }

        private void CheckIndexResponse(IElasticsearchResponse indexResp)
        {
            indexResp.ThrowIfInvalid("Can't index entities");

            var respStr = Encoding.UTF8.GetString(indexResp.ApiCall.ResponseBodyInBytes);
            if (respStr.Contains("\"errors\":true"))
            {
                throw new InvalidOperationException("Can't index entities")
                    .AndFactIs("Response", respStr);
            }
        }

        public async Task RemoveEntitiesAsync(string[] ids)
        {
            var resp = await _esManager.Client.DeleteByQueryAsync<DbEntity>(d => 
                d.Index(_indexOptions.IndexName)
                    .Query(qd => 
                        qd.Ids(idsQd => 
                            idsQd.Values(ids))));
            resp.ThrowIfInvalid("Can't remove entities by identifiers");
        }

        public IEntityReIndexer Reindex()
        {
            throw new NotImplementedException();
        }

        public static string GetRealIndexName(string originIndexName) => originIndexName + "-real";
    }
}