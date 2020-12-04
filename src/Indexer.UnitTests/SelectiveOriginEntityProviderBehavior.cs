using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Indexer;
using MyLab.Indexer.Common;
using Xunit;

namespace Indexer.UnitTests
{
    public class SelectiveOriginEntityProviderBehavior
    {
        [Fact]
        public async Task ShouldProvidePerPage()
        {
            //Arrange
            var src = new SimpleTestOriginEntitySource();

            var ids = src
                .Select(e => e.Id)
                .ToArray();
            
            var provider = new SelectiveOriginEntityProviderLogic(ids);

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
        public async Task ShouldProvideSelected()
        {
            //Arrange
            var src = new SimpleTestOriginEntitySource();

            var ids = src
                .Take(5)
                .Select(e => e.Id)
                .ToArray();

            var provider = new SelectiveOriginEntityProviderLogic(ids);
            var buffer = new List<OriginEntity>();

            //Act
            await foreach (var p in provider.Provide(src, 3))
            {
                buffer.AddRange(p);
            }

            //Assert
            Assert.Equal(ids, buffer.Select(e => e.Id));
        }
    }
}