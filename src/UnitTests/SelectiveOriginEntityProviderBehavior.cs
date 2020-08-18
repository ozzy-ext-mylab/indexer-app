﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Indexer.Tools;
using Xunit;

namespace UnitTests
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
            
            var provider = new SelectiveOriginEntityProvider(ids);

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

            var provider = new SelectiveOriginEntityProvider(ids);
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