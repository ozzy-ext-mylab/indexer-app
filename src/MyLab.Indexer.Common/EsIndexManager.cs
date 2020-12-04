using System.Threading.Tasks;
using MyLab.Elastic;

namespace MyLab.Indexer.Common
{
    class EsIndexManager : IIndexManager
    {
        private readonly IEsManager _esMgr;

        public EsIndexManager(IEsManager esMgr)
        {
            _esMgr = esMgr;
        }

        public async Task<IIndexer> CreateIndexerForExistentAsync(string indexName)
        {
            return new EsIndexer(_esMgr.ForIndex(indexName));
        }

        public async Task<IDeindexer> CreateDeindexerForExistentAsync(string indexName)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IIndexer> CreateIndexAsync(string indexName)
        {
            throw new System.NotImplementedException();
        }

        public async Task RemoveIndexAsync(string indexName)
        {
            throw new System.NotImplementedException();
        }

        public async Task AliasIndex(string alias, string indexName)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> IsIndexExistsAsync(string indexName)
        {
            throw new System.NotImplementedException();
        }

        public async Task<string[]> GetAliasIndices(string aliasName)
        {
            throw new System.NotImplementedException();
        }
    }

}