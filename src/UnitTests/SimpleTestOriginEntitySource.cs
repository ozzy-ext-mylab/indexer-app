using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LinqToDB;
using MyLab.Indexer.Tools;

namespace UnitTests
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