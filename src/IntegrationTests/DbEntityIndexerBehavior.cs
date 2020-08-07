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
    public class DbEntityIndexerBehavior : IClassFixture<EsIndexFixture<DbEntityIndexerBehavior.TestEntity>>
    {
        private readonly EsIndexFixture<TestEntity> _fxt;

        public DbEntityIndexerBehavior(
            EsIndexFixture<TestEntity> fxt,
            ITestOutputHelper output)
        {
            _fxt = fxt;
            _fxt.Output = output;
        }

        [Fact]
        public async Task ShouldIndexDocuments()
        {
            //Arrange
            var options = new IndexOptions()
            {
                IndexName = _fxt.Manager.IndexName
            };
            var indexer = new DbEntityIndexer(options, _fxt.Manager.Client.LowLevel);
            var testEnt = new DbEntity
            {
                Id = "foo",
                ExtendedProperties = new Dictionary<string, object>
                {
                    { nameof(TestEntity.Value), "bar" }
                }
            };

            //Act
            await indexer.Index(new []{ testEnt });

            await Task.Delay(1000);

            var res = await _fxt.Manager.SearchAsync<TestEntity>(d =>
                d.Term(r =>
                    r.Field(e => e.Value)
                        .Value("bar")));

            //Assert
            Assert.Equal(1, res.Count);
            Assert.Equal("foo", res.First().Id);
        }

        [ElasticsearchType(IdProperty = nameof(Id))]
        public class TestEntity
        {
            [Keyword(Name = "Id")]
            public string Id { get; set; }

            [Keyword(Name = "Value")]
            public string Value { get; set; }
        }
    }
}
