using System.Threading.Tasks;
using MyLab.Elastic;

namespace MyLab.Indexer.Tools
{
    class EsDeindexer : IDeindexer
    {
        private readonly IIndexSpecificEsManager _mgr;

        public EsDeindexer(IIndexSpecificEsManager mgr)
        {
            _mgr = mgr;
        }

        public async Task DeindexAsync(string[] ids)
        {
            var bulkResponse = await _mgr.Client.BulkAsync(descriptor => descriptor
                .Index(_mgr.IndexName)
                .DeleteMany(ids)
            );

            bulkResponse.ThrowIfInvalid("Can't delete entities from index");
        }
    }
}