using System.Threading.Tasks;

namespace MyLab.Indexer.Services
{
    interface IEntityIndexer
    {
        Task IndexEntityBatchAsync(DbEntity[] docBatch);

        Task RemoveEntitiesAsync(string[] ids);
    }
}