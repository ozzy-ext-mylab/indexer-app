using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.Indexer;
using MyLab.Indexer.Common;
using MyLab.Indexer.Reindexer;
using Xunit;

namespace Reindexer.UnitTests
{
    public class AllOriginEntityProviderBehavior
    {
        [Fact]
        public async Task ShouldProvidePerPage()
        {
            //Arrange
            var src = new SimpleTestOriginEntitySource();
            var provider = new AllOriginEntityProviderLogic();
            OriginEntity[] page = null;

            //Act
            await foreach (var p in provider.Provide(src, 3))
            {
                page = p;
                break;
            }

            //Assert
            Assert.NotNull(page);
            Assert.Equal(3, page.Length);
        }

        [Fact]
        public async Task ShouldProvideAll()
        {
            //Arrange
            var src = new SimpleTestOriginEntitySource();
            var provider = new AllOriginEntityProviderLogic();
            var buffer = new List<OriginEntity>();

            //Act
            await foreach (var p in provider.Provide(src, 3))
            {
                buffer.AddRange(p);
            }

            //Assert
            Assert.Equal(src, buffer);
        }
    }
}
