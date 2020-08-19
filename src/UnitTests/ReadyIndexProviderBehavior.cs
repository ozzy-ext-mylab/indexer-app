﻿using System.Threading.Tasks;
using Moq;
using MyLab.Indexer.Tools;
using Xunit;

namespace UnitTests
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
                .Setup(m => m.CreateIndexerForExistent(It.IsAny<string>()))
                .Returns(() => Task.FromResult(indexerOrigin));
            
            indexMgr
                .Setup(m => m.IsIndexExists(It.IsAny<string>()))
                .Returns(() => Task.FromResult(true));

            var provider = new ReadyIndexProvider(indexMgr.Object, "foo");

            //Act
            var indexer = await provider.ProvideAsync();

            //Assert

            indexMgr.Verify(m => m.IsIndexExists(It.Is<string>(s => s == "foo")));
            indexMgr.Verify(m => m.CreateIndexerForExistent(It.Is<string>(s => s == "foo")));
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
                .Setup(m => m.CreateIndexerForExistent(It.IsAny<string>()))
                .Returns(() => Task.FromResult(indexerOrigin));

            indexMgr
                .Setup(m => m.IsIndexExists(It.Is<string>(s => s == "foo")))
                .Returns(() => Task.FromResult(false));

            var provider = new ReadyIndexProvider(indexMgr.Object, "foo");

            //Act
            var indexer = await provider.ProvideAsync();

            //Assert

            indexMgr.Verify(m => m.IsIndexExists(It.Is<string>(s => s == "foo")));
            indexMgr.Verify(m => m.CreateIndex(It.IsAny<string>()));
            indexMgr.Verify(m => m.CreateAlias(It.Is<string>(s => s == "foo"),It.IsAny<string>()));
            indexMgr.Verify(m => m.CreateIndexerForExistent(It.Is<string>(s => s == "foo")));
            indexMgr.VerifyNoOtherCalls();

            Assert.Equal(indexerOrigin, indexer);
        }
    }
}
