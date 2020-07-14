using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace MyLab.Indexer.Services
{
    class DbEntityIndexer
    {
        private readonly string _indexName;
        private readonly IElasticLowLevelClient _client;

        public DbEntityIndexer(string indexName, IElasticLowLevelClient client)
        {
            _indexName = indexName ?? throw new ArgumentNullException(nameof(indexName));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IElasticsearchResponse> Index(IEnumerable<DbEntity> entities)
        {
            return await _client.BulkAsync<BytesResponse>(_indexName, CreateRequest(entities));
        }

        string CreateRequest(IEnumerable<DbEntity> entities)
        {
            var sb = new StringBuilder();

            foreach (var entity in entities)
            {
                sb.AppendLine("{\"index\":{}}");

                var docBuilder = new IndexDocumentBuilder(entity);
                sb.AppendLine(docBuilder.BuildJson());
            }

            return sb.ToString();
        }
    }
}