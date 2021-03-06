﻿using System.Threading.Tasks;
using Moq;
using MyLab.Indexer;
using MyLab.Indexer.Common;
using Xunit;

namespace Indexer.UnitTests
{
    public class ReadyIndexProviderBehavior
    {
        [Fact]
        public async Task ShouldUseExistentIndexIfExists()
        {
            //Arrange
            var indexerOrigin = new Moq.Mock<IIndexer>().Object;
            var indexMgr = new Moq.Mock<IIndexManager>();

            indexMgr
                .Setup(m => m.CreateIndexerForExistentAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(indexerOrigin));
            
            indexMgr
                .Setup(m => m.IsIndexExistsAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(true));

            var provider = new ReadyIndexProvider("foo", indexMgr.Object);

            //Act
            var indexer = await provider.ProvideIndexerAsync();

            //Assert

            indexMgr.Verify(m => m.IsIndexExistsAsync(It.Is<string>(s => s == "foo")));
            indexMgr.Verify(m => m.CreateIndexerForExistentAsync(It.Is<string>(s => s == "foo")));
            indexMgr.VerifyNoOtherCalls();

            Assert.Equal(indexerOrigin, indexer);
        }

        [Fact]
        public async Task ShouldCreateEvenIndexAndAliasIfIndexNotExists()
        {
            //Arrange
            var indexerOrigin = new Moq.Mock<IIndexer>().Object;
            var indexMgr = new Moq.Mock<IIndexManager>();

            indexMgr
                .Setup(m => m.CreateIndexerForExistentAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(indexerOrigin));

            indexMgr
                .Setup(m => m.IsIndexExistsAsync(It.Is<string>(s => s == "foo")))
                .Returns(() => Task.FromResult(false));

            var provider = new ReadyIndexProvider("foo", indexMgr.Object);

            //Act
            var indexer = await provider.ProvideIndexerAsync();

            //Assert

            indexMgr.Verify(m => m.IsIndexExistsAsync(It.Is<string>(s => s == "foo")));
            indexMgr.Verify(m => m.CreateIndexAsync(It.IsAny<string>()));
            indexMgr.Verify(m => m.AliasIndex(It.Is<string>(s => s == "foo"),It.IsAny<string>()));
            indexMgr.Verify(m => m.CreateIndexerForExistentAsync(It.Is<string>(s => s == "foo")));
            indexMgr.VerifyNoOtherCalls();

            Assert.Equal(indexerOrigin, indexer);
        }
    }
}
