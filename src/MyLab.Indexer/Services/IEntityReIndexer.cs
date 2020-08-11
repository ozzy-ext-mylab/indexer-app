using System;
using System.Threading.Tasks;
using MyLab.Db;
using MyLab.Elastic;

namespace MyLab.Indexer.Services
{
    interface IEntityReIndexer : IDisposable
    {
        Task IndexEntityBatchAsync(DbEntity[] docBatch);
        Task Commit();

        Task Rollback();
    }

    class DefaultEntityReIndexer : IEntityReIndexer
    {
        private readonly IEsManager _esMgr;

        public DefaultEntityReIndexer(IEsManager esMgr)
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
    }
}