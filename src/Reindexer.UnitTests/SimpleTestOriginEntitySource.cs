﻿using System.Collections.Generic;
using System.Linq;
using MyLab.Indexer;
using MyLab.Indexer.Common;

namespace Reindexer.UnitTests
{
    class SimpleTestOriginEntitySource : List<OriginEntity>, IOriginEntitySource
    {
        public SimpleTestOriginEntitySource()
        {
            for (int i = 0; i < 10; i++)
            {
                Add(new OriginEntity{Id = i.ToString()});
            }
        }

        public IQueryable<OriginEntity> Query()
        {
            return this.AsQueryable();
        }
    }
}