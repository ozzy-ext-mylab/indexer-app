using System;
using System.Threading.Tasks;
using MyLab.Db;

namespace MyLab.Indexer.Services
{
    interface IEntityReIndexer : IDisposable
    {
        Task IndexEntityBatchAsync(DbEntity[] docBatch);
        Task Commit();

        Task Rollback();
    }
}