using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyLab.Db;
using MyLab.Indexer.Common;
using MyLab.TaskApp;

namespace MyLab.Indexer.Reindexer
{
    class ReindexerTaskLogic : ITaskLogic
    {
        private readonly IIndexManager _indexManager;
        private readonly IOriginEntitySource _originEntitySource;
        private readonly ReindexerOptions _options;

        public ReindexerTaskLogic(
            IIndexManager indexManager,
            IOriginEntitySource originEntitySource,
            IOptions<ReindexerOptions> options)
            :this(indexManager, originEntitySource, options.Value)
        {

        }

        public ReindexerTaskLogic(
            IIndexManager indexManager,
            IOriginEntitySource originEntitySource,
            ReindexerOptions options)
        {
            _indexManager = indexManager;
            _originEntitySource = originEntitySource;
            _options = options;
        }

        public async Task Perform(CancellationToken cancellationToken)
        {
            await using var reindexer = await ReindexOperator.StartReindexingAsync(_options.IndexName, _indexManager);

            var entityProvider = new OriginEntityProvider(
                new AllOriginEntityProviderLogic(),
                _originEntitySource,
                _options.PageSize
            );

            var entityIndexer = new EntityIndexer(reindexer, entityProvider);

            await entityIndexer.IndexAsync();
            await reindexer.CommitAsync();
        }
    }
}