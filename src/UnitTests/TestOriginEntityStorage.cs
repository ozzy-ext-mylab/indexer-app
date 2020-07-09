using System.Collections.ObjectModel;
using System.Linq;
using MyLab.Indexer.Services;

namespace UnitTests
{
    class TestOriginEntityStorage : Collection<DbEntity>, IOriginEntityStorage
    {
        public TestOriginEntityStorage()
            :this(10)
        {

        }

        public TestOriginEntityStorage(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Add(new DbEntity { Id = i.ToString() });
            }
        }

        public IQueryable<DbEntity> Query()
        {
            return this.AsQueryable();
        }
    }
}