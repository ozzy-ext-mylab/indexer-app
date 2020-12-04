using System.Threading.Tasks;
using Moq;
using MyLab.Indexer;
using MyLab.Indexer.Common;
using Xunit;

namespace Indexer.UnitTests
{
    public class ExistentDeindexerProviderBehavior
    {
        [Fact]
        public async Task ShouldProvideDeindexerIfIndexExists()
        {
            //Arrange

            var originDeindexer = new Mock<IDeindexer>();
            var indexMgr = new Mock<IIndexManager>();

            indexMgr
                .Setup(m => m.IsIndexExistsAsync(It.Is<string>(s => s == "foo")))
                .Returns(() => Task.FromResult(true));

            indexMgr
                .Setup(m => m.CreateDeindexerForExistentAsync(It.Is<string>(s => s == "foo")))
                .Returns(() => Task.FromResult(originDeindexer.Object));

            var provider = new ExistentDeindexerProvider("foo", indexMgr.Object);

            //Act
            var deindexer = await provider.ProvideAsync();

            //Assert
            Assert.NotNull(deindexer);
            
            indexMgr.Verify(m => m.IsIndexExistsAsync(It.Is<string>(s => s == "foo")));
            indexMgr.Verify(m => m.CreateDeindexerForExistentAsync(It.Is<string>(s => s == "foo")));
            indexMgr.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldProvideNullIfIndexDoesNotExist()
        {
            //Arrange

            var originDeindexer = new Mock<IDeindexer>();
            var indexMgr = new Mock<IIndexManager>();

            indexMgr
                .Setup(m => m.IsIndexExistsAsync(It.Is<string>(s => s == "foo")))
                .Returns(() => Task.FromResult(false));

            var provider = new ExistentDeindexerProvider("foo", indexMgr.Object);

            //Act
            var deindexer = await provider.ProvideAsync();

            //Assert
            Assert.Null(deindexer);

            indexMgr.Verify(m => m.IsIndexExistsAsync(It.Is<string>(s => s == "foo")));
            indexMgr.VerifyNoOtherCalls();
        }
    }
}
