using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Elastic;
using MyLab.Elastic.Test;
using MyLab.Indexer;
using MyLab.Indexer.Services;
using Nest;
using Xunit;
using Xunit.Abstractions;
using IndexOptions = MyLab.Indexer.IndexOptions;

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
            var indexerOptions = new IndexOptions
            {
                IndexName = indexName,
                IdFieldName = nameof(TestEntity.CustomId),
                IdFieldIsString = false
            };
            var indexer = new DefaultEntityIndexer(_fxt.Manager,indexerOptions);
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
                    d.Ids(qd => qd.Values(2)));
            }
            finally
            {
                await _fxt.Manager.Client.Indices.DeleteAsync(indexName);
            }

            var foundOne = found?.FirstOrDefault();

            //Assert
            Assert.NotNull(found);
            Assert.Equal(1, found.Count);
            Assert.NotNull(foundOne);
            Assert.Equal(2, foundOne.CustomId);
            Assert.Equal("bar", foundOne.Value);

        }

        [ElasticsearchType(IdProperty = nameof(Id))]
        public class TestEntity
        {
            [Text(Name = "CustomId")]
            public int CustomId { get; set; }

            [Text(Name = "Value")]
            public string Value { get; set; }

            
        }
    }
}
