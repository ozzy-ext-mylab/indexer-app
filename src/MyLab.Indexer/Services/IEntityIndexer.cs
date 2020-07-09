using System.Threading.Tasks;

namespace MyLab.Indexer.Services
{
    interface IEntityIndexer
    {
        Task IndexEntityBatchAsync(DbEntity[] docBatch);

        Task RemoveEntitiesAsync(string[] ids);

        Task StartReindex();

        Task EndReindex();
    }

    class DefaultEntityIndexer : IEntityIndexer
    {
        public Task IndexEntityBatchAsync(DbEntity[] docBatch)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveEntitiesAsync(string[] ids)
        {
            throw new System.NotImplementedException();
        }

        public Task StartReindex()
        {
            throw new System.NotImplementedException();
        }

        public Task EndReindex()
        {
            throw new System.NotImplementedException();
        }
    }
}