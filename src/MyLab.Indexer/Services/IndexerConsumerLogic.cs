using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Indexer.Tools;
using MyLab.LogDsl;
using MyLab.Mq;

namespace MyLab.Indexer.Services
{
    class IndexerConsumerLogic : IMqConsumerLogic<IndexingMsg>
    {
        private readonly IOriginEntitySource _originEntitySource;
        private readonly IIndexManager _indexManager;
        private readonly IndexAppOptions _options;
        private readonly DslLogger _log;

        public IndexerConsumerLogic(
            IOriginEntitySource originEntitySource,
            IIndexManager indexManager,
            IndexAppOptions options,
            ILogger<IndexerConsumerLogic> logger)
        
        {
            _originEntitySource = originEntitySource ?? throw new ArgumentNullException(nameof(originEntitySource));
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _log = logger?.Dsl();
        }

        public IndexerConsumerLogic(
            IOriginEntitySource originEntitySource,
            IIndexManager indexManager,
            IOptions<IndexAppOptions> options,
            ILogger<IndexerConsumerLogic> logger)
            : this(originEntitySource, indexManager, options.Value, logger)
        {
            
        }

        public async Task Consume(MqMessage<IndexingMsg> message)
        {
            if (message.Payload == null)
            {
                _log?.Warning("Message payload is null")
                    .AndFactIs("message-id", message.MessageId)
                    .Write();
                return;
            }

            if (message.Payload.ReindexRequired)
            {
                await ReindexAsync();
            }
            else
            {
                if(message.Payload.DeindexList != null)
                    await DeindexAsync(message.Payload.DeindexList);
                if (message.Payload.IndexList != null)
                    await IndexAsync(message.Payload.IndexList);
            }
        }

        private async Task IndexAsync(string[] ids)
        {
            var readyIndexProvider = new ReadyIndexProvider(_options.EsAlias, _indexManager);
            
            var entityProvider = new OriginEntityProvider(
                new SelectiveOriginEntityProviderLogic(ids), 
                _originEntitySource,
                _options.PageSize
                );

            var entityIndexer = new EntityIndexer(readyIndexProvider, entityProvider);

            await entityIndexer.IndexAsync();
        }

        private async Task DeindexAsync(string[] ids)
        {
            var deindexerProvider = new ExistentDeindexerProvider(_options.EsAlias, _indexManager);
            var deindexer = await deindexerProvider.ProvideAsync();
            await deindexer.DeindexAsync(ids);
        }

        private async Task ReindexAsync()
        {
            await using var reindexer = await ReindexOperator.StartReindexingAsync(_options.EsAlias, _indexManager);

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

    class IndexAppOptions
    {
        public string EsAlias { get; set; }
        public int PageSize { get; set; } = 100;
    }
}
