using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;

namespace MyLab.Indexer.Services
{
    class DbEntityIndexer
    {
        private readonly IndexOptions _options;
        private readonly IElasticLowLevelClient _client;

        public DbEntityIndexer(IndexOptions options, IElasticLowLevelClient client)
        {
            _options = options;
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IElasticsearchResponse> Index(IEnumerable<DbEntity> entities)
        {
            return await _client.BulkAsync<BytesResponse>(_options.IndexName, CreateRequest(entities));
        }

        string CreateRequest(IEnumerable<DbEntity> entities)
        {
            var sb = new StringBuilder();

            foreach (var entity in entities)
            {
                var strIdPattern = _options.IdFieldIsString  ? "\"{0}\"" : "{0}";
                var strId = string.Format(strIdPattern, entity.Id);

                sb.AppendLine("{\"index\":{ \"_id\":" + strId +  "}}");

                var props = new Dictionary<string, object>(entity.ExtendedProperties);

                var strJsonProps = props.Select(p => $"\"{p.Key}\":\"{p.Value}\"").ToList();
                strJsonProps.Add($"\"{_options.IdFieldName ?? nameof(DbEntity.Id)}\":{strId}");

                sb.AppendLine("{" + string.Join(',', strJsonProps) + "}");
            }

            return sb.ToString();
        }
    }
}