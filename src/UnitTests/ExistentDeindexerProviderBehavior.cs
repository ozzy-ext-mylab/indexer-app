﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyLab.Indexer.Tools;
using Xunit;

namespace UnitTests
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
                .Setup(m => m.IsIndexExists(It.Is<string>(s => s == "foo")))
                .Returns(() => Task.FromResult(true));

            indexMgr
                .Setup(m => m.CreateDeindexerForExistent(It.Is<string>(s => s == "foo")))
                .Returns(() => Task.FromResult(originDeindexer.Object));

            var provider = new ExistentDeindexerProvider("foo", indexMgr.Object);

            //Act
            var deindexer = await provider.Provide();

            //Assert
            Assert.NotNull(deindexer);
            
            indexMgr.Verify(m => m.IsIndexExists(It.Is<string>(s => s == "foo")));
            indexMgr.Verify(m => m.CreateDeindexerForExistent(It.Is<string>(s => s == "foo")));
            indexMgr.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldProvideNullIfIndexDoesNotExist()
        {
            //Arrange

            var originDeindexer = new Mock<IDeindexer>();
            var indexMgr = new Mock<IIndexManager>();

            indexMgr
                .Setup(m => m.IsIndexExists(It.Is<string>(s => s == "foo")))
                .Returns(() => Task.FromResult(false));

            var provider = new ExistentDeindexerProvider("foo", indexMgr.Object);

            //Act
            var deindexer = await provider.Provide();

            //Assert
            Assert.Null(deindexer);

            indexMgr.Verify(m => m.IsIndexExists(It.Is<string>(s => s == "foo")));
            indexMgr.VerifyNoOtherCalls();
        }
    }
}