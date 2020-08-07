using System;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Connections.Features;
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
        private readonly string _indexName;

        public DefaultEntityIndexer(IEsManager esManager, IOptions<IndexerOptions> options)
            :this(esManager, options.Value.Index)
        {
        }

        public DefaultEntityIndexer(IEsManager esManager, string indexName)
        {
            _esManager = esManager;
            _indexName = indexName;
        }

        public async Task IndexEntityBatchAsync(DbEntity[] docBatch)
        {
            var exRes = await _esManager.Client.Indices.ExistsAsync(_indexName);
            exRes.ThrowIfInvalid("Can't check index for existing");

            var realIndex = GetRealIndexName();

            if (!exRes.Exists)
            {
                var crIndexResp = await _esManager.Client.Indices.CreateAsync(realIndex);
                crIndexResp.ThrowIfInvalid("Can't create index");

                var crAliasResp = await _esManager.Client.Indices.PutAliasAsync(realIndex, _indexName);
                crAliasResp.ThrowIfInvalid("Can't create alias for index");
            }

            var indexer = new DbEntityIndexer(_indexName, _esManager.Client.LowLevel);

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

        string GetRealIndexName() => _indexName + "-real";
    }

    interface IIndexMappingProvider
    {

    }
}