using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MyLab.Elastic;
using MyLab.Elastic.Test;
using MyLab.Indexer.Services;
using Nest;
using Utf8Json;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class DefaultEntityIndexerBehavior : IClassFixture<EsFixture>
    {
        private readonly EsFixture _fxt;

        public DefaultEntityIndexerBehavior(EsFixture fxt, ITestOutputHelper output)
        {
            _fxt = fxt;
            _fxt.Output = output;
        }

        [Fact]
        public async Task ShouldIndexInExistentIndex()
        {
            //Arrange
            var indexName = "test-" + Guid.NewGuid().ToString("N");
            var indexer = new DefaultEntityIndexer(_fxt.Manager, indexName);
            var testEnt = new DbEntity
            {
                Id = "2",
                ExtendedProperties = new Dictionary<string, object>
                {
                    { nameof(TestEntity.Value), "bar" }
                }
            };

            var createIndexResp = await _fxt.Manager.Client.Indices.CreateAsync(indexName,
                cd => cd.Map<TestEntity>(md => md.AutoMap()));
            createIndexResp.ThrowIfInvalid("Can't create index");

            IReadOnlyCollection<TestEntity> found = null;

            //Act

            try
            {

                await indexer.IndexEntityBatchAsync(new[] {testEnt});
                await Task.Delay(1000);

                found = await _fxt.Manager.SearchAsync<TestEntity>(indexName, d =>
                    d.Ids(qd => qd.Values("2")));
            }
            finally
            {
                await _fxt.Manager.Client.Indices.DeleteAsync(indexName);
            }

            //Assert
            Assert.NotNull(found);
            Assert.Equal(1, found.Count);

        }

        [ElasticsearchType(IdProperty = nameof(Id))]
        public class TestEntity
        {
            [Text]
            public string Id { get; set; }

            [Text]
            public string Value { get; set; }

            
        }
    }
}
