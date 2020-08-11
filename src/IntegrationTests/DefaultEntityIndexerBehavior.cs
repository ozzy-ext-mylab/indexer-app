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
            var opt = GenerateIndexName();
            var indexer = new DefaultEntityIndexManager(_fxt.Manager,opt);
            var testEnt = CreateDnEnt("2", "bar");
            
            await CreateIndexAsync(opt.IndexName);

            IReadOnlyCollection<TestEntity> found = null;

            //Act

            try
            {
                await IndexItemsAsync(indexer, testEnt);

                found = await _fxt.Manager.SearchAsync<TestEntity>(opt.IndexName, d =>
                    d.Ids(qd => qd.Values(2)));
            }
            finally
            {
                await _fxt.Manager.Client.Indices.DeleteAsync(opt.IndexName);
            }

            var foundOne = found?.FirstOrDefault();

            //Assert
            Assert.NotNull(found);
            Assert.Equal(1, found.Count);
            Assert.NotNull(foundOne);
            Assert.Equal(2, foundOne.CustomId);
            Assert.Equal("bar", foundOne.Value);

        }

        [Fact]
        public async Task ShouldRemoveEntitiesByIds()
        {
            //Arrange
            var opt = GenerateIndexName();
            var indexer = new DefaultEntityIndexManager(_fxt.Manager, opt);
            var entities = new[]
            {
                CreateDnEnt("0", "foo"),
                CreateDnEnt("1", "bar"),
                CreateDnEnt("2", "baz")
            };

            await CreateIndexAsync(opt.IndexName);
            await IndexItemsAsync(indexer, entities);

            IReadOnlyCollection<TestEntity> found = null;

            //Act

            try
            {
                await indexer.RemoveEntitiesAsync(new [] {"1", "2"});
                await WaitingForEsAsync();

                found = await _fxt.Manager.SearchAsync<TestEntity>(opt.IndexName,
                    d => d.MatchAll());
            }
            finally
            {
                await _fxt.Manager.Client.Indices.DeleteAsync(opt.IndexName);
            }

            var foundOne = found?.FirstOrDefault();

            //Assert
            Assert.NotNull(found);
            Assert.Equal(1, found.Count);
            Assert.NotNull(foundOne);
            Assert.Equal(0, foundOne.CustomId);
            Assert.Equal("foo", foundOne.Value);
        }

        async Task IndexItemsAsync(DefaultEntityIndexManager defaultEntityIndexManager, params DbEntity[] entities)
        {
            await defaultEntityIndexManager.IndexEntityBatchAsync(entities);
            await WaitingForEsAsync();
        }

        private static async Task WaitingForEsAsync()
        {
            await Task.Delay(1000);
        }

        async Task CreateIndexAsync(string indexName)
        {
            var createIndexResp = await _fxt.Manager.Client.Indices.CreateAsync(indexName,
                cd => cd.Map<TestEntity>(md => md.AutoMap()));
            createIndexResp.ThrowIfInvalid("Can't create index");
        }

        IndexOptions GenerateIndexName()
        {
            var indexNm = "test-" + Guid.NewGuid().ToString("N");

            return new IndexOptions
            {
                IndexName = indexNm,
                IdFieldIsString = false,
                IdFieldName = nameof(TestEntity.CustomId)
            };
        }

        DbEntity CreateDnEnt(string id, string value)
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
            [Text(Name = "CustomId")]
            public int CustomId { get; set; }

            [Text(Name = "Value")]
            public string Value { get; set; }

            
        }
    }
}
