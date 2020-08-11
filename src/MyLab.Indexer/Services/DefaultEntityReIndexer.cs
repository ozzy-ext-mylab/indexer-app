using System;
using System.Threading.Tasks;
using MyLab.Elastic;

namespace MyLab.Indexer.Services
{
    class DefaultEntityReIndexer : IEntityReIndexer
    {
        private readonly IEsManager _esMgr;

        public DefaultEntityReIndexer(string indexName, IEsManager esMgr)
        {
            _esMgr = esMgr;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task IndexEntityBatchAsync(DbEntity[] docBatch)
        {
            throw new NotImplementedException();
        }

        public Task Commit()
        {
            throw new NotImplementedException();
        }

        public Task Rollback()
        {
            throw new NotImplementedException();
        }

        public Task Begin()
        {
            throw new NotImplementedException();
        }
    }
}