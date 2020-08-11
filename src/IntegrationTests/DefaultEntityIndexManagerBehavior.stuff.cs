using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.Elastic;
using MyLab.Elastic.Test;
using MyLab.Indexer.Services;
using Nest;
using Xunit.Abstractions;
using IndexOptions = MyLab.Indexer.IndexOptions;

namespace IntegrationTests
{
    public partial class DefaultEntityIndexManagerBehavior
    {
        private readonly EsFixture _fxt;

        public DefaultEntityIndexManagerBehavior(EsFixture fxt, ITestOutputHelper output)
        {
            _fxt = fxt;
            _fxt.Output = output;
        }

        private static async Task WaitingForEsAsync()
        {
            await Task.Delay(1000);
        }

        private async Task CreateIndexAsync(string indexName)
        {
            var createIndexResp = await _fxt.Manager.Client.Indices.CreateAsync(indexName,
                cd => cd.Map<TestEntity>(md => md.AutoMap()));
            createIndexResp.ThrowIfInvalid("Can't create index");
        }

        private IndexOptions GenerateIndexName()
        {
            var indexNm = "test-" + Guid.NewGuid().ToString("N");

            return new IndexOptions
            {
                IndexName = indexNm,
                IdFieldIsString = false,
                IdFieldName = nameof(TestEntity.CustomId)
            };
        }

        private DbEntity CreateDnEnt(string id, string value)
        {
            return new DbEntity
            {
                Id = id,
                ExtendedProperties = new Dictionary<string, object>
                {
                    {nameof(TestEntity.Value), value}
                }
            };
        }

        [ElasticsearchType(IdProperty = nameof(Id))]
        public class TestEntity
        {
            [Text(Name = "CustomId")] public int CustomId { get; set; }

            [Text(Name = "Value")] public string Value { get; set; }
        }
    }
}