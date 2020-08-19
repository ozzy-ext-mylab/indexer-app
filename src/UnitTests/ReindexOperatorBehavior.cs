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

            indexMgr.Verify(m => m.CreateIndex(It.IsAny<string>()));
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
            indexMgr.Verify(m => m.RemoveIndex(It.Is<string>(s => s == reindexer.IndexName)));
        }

        [Fact]
        public async Task ShouldCreateIndexerForNewIndex()
        {
            //Arrange
            var expectedIndexer = new Mock<IIndexer>();
            var indexMgr = new Moq.Mock<IIndexManager>();
            var reindexer = await ReindexOperator.StartReindexingAsync("foo", indexMgr.Object);

            indexMgr
                .Setup(m => m.CreateIndexerForExistent(It.Is<string>(s => s == reindexer.IndexName)))
                .Returns(() => Task.FromResult(expectedIndexer.Object));

            //Act
            var indexer = await reindexer.GetIndexerAsync();
            
            //Assert
            Assert.Equal(expectedIndexer.Object, indexer);
        }
    }
}
