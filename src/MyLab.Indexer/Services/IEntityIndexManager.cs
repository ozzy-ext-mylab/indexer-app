using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using Nest;

namespace MyLab.Indexer.Services
{
    interface IEntityIndexManager
    {
        Task IndexEntityBatchAsync(DbEntity[] docBatch);

        Task RemoveEntitiesAsync(string[] ids);

        IEntityReIndexer Reindex();
    }
}