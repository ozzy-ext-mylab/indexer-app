using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Elastic;
using MyLab.Elastic.Test;
using MyLab.Indexer.Services;
using Xunit;

namespace IntegrationTests
{
    public partial class DefaultEntityIndexManagerBehavior : IClassFixture<EsFixture>
    {
        [Fact]
        public async Task ShouldIndexInExistentIndex()
        {
            //Arrange
            var opt = GenerateIndexName();
            var mgr = new DefaultEntityIndexManager(_fxt.Manager,opt);
            var testEnt = CreateDnEnt("2", "bar");
            
            await CreateIndexAsync(opt.IndexName);

            IReadOnlyCollection<TestEntity> found = null;

            //Act

            try
            {
                await mgr.IndexEntityBatchAsync(new []{testEnt});
                await WaitingForEsAsync();

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
        public async Task ShouldCreateIndexIfNotExists()
        {
            //Arrange
            var opt = GenerateIndexName();
            var mgr = new DefaultEntityIndexManager(_fxt.Manager, opt);
            var testEnt = CreateDnEnt("2", "bar");

            IReadOnlyCollection<TestEntity> found;

            //Act

            try
            {
                await mgr.IndexEntityBatchAsync(new[] { testEnt });
                await WaitingForEsAsync();

                found = await _fxt.Manager.SearchAsync<TestEntity>(opt.IndexName, d =>
                    d.Ids(qd => qd.Values(2)));
            }
            finally
            {
                await _fxt.Manager.Client.Indices.DeleteAsync(DefaultEntityIndexManager.GetRealIndexName(opt.IndexName));
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
        public async Task ShouldUseEarlyCreatedIndex()
        {
            //Arrange
            var opt = GenerateIndexName();
            var mgr = new DefaultEntityIndexManager(_fxt.Manager, opt);
            var testEnt1 = CreateDnEnt("1", "foo");
            var testEnt2 = CreateDnEnt("2", "bar");

            IReadOnlyCollection<TestEntity> found;

            //Act

            try
            {
                await mgr.IndexEntityBatchAsync(new[] { testEnt1 });
                await mgr.IndexEntityBatchAsync(new[] { testEnt2 });
                await WaitingForEsAsync();

                found = await _fxt.Manager.SearchAsync<TestEntity>(opt.IndexName, d =>
                    d.Ids(qd => qd.Values(2)));
            }
            finally
            {
                await _fxt.Manager.Client.Indices.DeleteAsync(DefaultEntityIndexManager.GetRealIndexName(opt.IndexName));
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
            var mgr = new DefaultEntityIndexManager(_fxt.Manager, opt);
            var entities = new[]
            {
                CreateDnEnt("0", "foo"),
                CreateDnEnt("1", "bar"),
                CreateDnEnt("2", "baz")
            };

            await CreateIndexAsync(opt.IndexName);
            await mgr.IndexEntityBatchAsync(entities);
            await WaitingForEsAsync();

            IReadOnlyCollection<TestEntity> found = null;

            //Act

            try
            {
                await mgr.RemoveEntitiesAsync(new [] {"1", "2"});
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
    }
}
