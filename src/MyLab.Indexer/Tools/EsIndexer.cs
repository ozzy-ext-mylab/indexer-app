using System.Threading.Tasks;
using MyLab.Elastic;

namespace MyLab.Indexer.Tools
{
    class EsIndexer : IIndexer
    {
        private readonly IIndexSpecificEsManager _mgr;

        public EsIndexer(IIndexSpecificEsManager mgr)
        {
            _mgr = mgr;
        }

        public async Task IndexAsync(OriginEntity[] entities)
        {
            await _mgr.IndexManyAsync(entities);
        }
    }
}