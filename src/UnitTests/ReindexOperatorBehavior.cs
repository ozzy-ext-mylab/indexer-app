using System.Threading.Tasks;
using Moq;
using MyLab.Indexer.Tools;
using Xunit;

namespace UnitTests
{
    public class ReindexOperatorBehavior
    {
        [Fact]
        public async Task ShouldCreateNewIndexForReindexingAtTheStart()
        {
            //Arrange
            var indexMgr = new Moq.Mock<IIndexManager>();

            //Act
            var reindexer = await ReindexOperator.StartReindexingAsync("foo", indexMgr.Object);

            //Assert
            Assert.NotNull(reindexer);

            indexMgr.Verify(m => m.CreateIndexAsync(It.IsAny<string>()));
            indexMgr.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRemoveNewIndexWhenDispose()
        {
            //Arrange
            var indexMgr = new Moq.Mock<IIndexManager>();
            var reindexer = await ReindexOperator.StartReindexingAsync("foo", indexMgr.Object);

            //Act

            await reindexer.DisposeAsync();

            //Assert
            indexMgr.Verify(m => m.RemoveIndexAsync(It.Is<string>(s => s == reindexer.IndexName)));
        }

        [Fact]
        public async Task ShouldCreateIndexerForNewIndex()
        {
            //Arrange
            var expectedIndexer = new Mock<IIndexer>();
            var indexMgr = new Moq.Mock<IIndexManager>();
            var reindexer = await ReindexOperator.StartReindexingAsync("foo", indexMgr.Object);

            indexMgr
                .Setup(m => m.CreateIndexerForExistentAsync(It.Is<string>(s => s == reindexer.IndexName)))
                .Returns(() => Task.FromResult(expectedIndexer.Object));

            //Act
            var indexer = await reindexer.GetIndexerAsync();
            
            //Assert
            Assert.Equal(expectedIndexer.Object, indexer);
        }

        [Fact]
        public async Task ShouldReplaceIndicesWhenReindexCommit()
        {
            //Arrange
            var indexMgr = new Moq.Mock<IIndexManager>();
            var reindexer = await ReindexOperator.StartReindexingAsync("foo", indexMgr.Object);

            indexMgr
                .Setup(m => m.GetAliasIndices(It.Is<string>(s => s == "foo")))
                .Returns(() => Task.FromResult(new [] {"bar"}));

            indexMgr
                .Setup(m => m.IsIndexExistsAsync(It.Is<string>(s => s == "foo")))
                .Returns(() => Task.FromResult(true));

            //Act
            await reindexer.CommitAsync();

            //Assert
            indexMgr.Verify(m => m.AliasIndex(
                It.Is<string>(s => s == "foo"),
                It.Is<string>(s => s == reindexer.IndexName)));
            indexMgr.Verify(m => m.RemoveIndexAsync(It.Is<string>(s => s == "bar")));
        }
    }
}
