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
        private readonly IndexAppEsOptions _esOptions;
        private readonly IndexAppLogicOptions _logicOptions;
        private readonly DslLogger _log;

        public IndexerConsumerLogic(
            IOriginEntitySource originEntitySource,
            IIndexManager indexManager,
            IndexAppEsOptions esOptions,
            IndexAppLogicOptions logicOptions,
            ILogger<IndexerConsumerLogic> logger)
        
        {
            _originEntitySource = originEntitySource ?? throw new ArgumentNullException(nameof(originEntitySource));
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));
            _esOptions = esOptions ?? throw new ArgumentNullException(nameof(esOptions));
            _logicOptions = logicOptions ?? throw new ArgumentNullException(nameof(logicOptions));
            _log = logger?.Dsl();
        }

        public IndexerConsumerLogic(
            IOriginEntitySource originEntitySource,
            IIndexManager indexManager,
            IOptions<IndexAppEsOptions> esOptions,
            IOptions<IndexAppLogicOptions> logicOptions,
            ILogger<IndexerConsumerLogic> logger)
            : this(originEntitySource, indexManager, esOptions.Value, logicOptions.Value, logger)
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
            var readyIndexProvider = new ReadyIndexProvider(_esOptions.IndexName, _indexManager);
            
            var entityProvider = new OriginEntityProvider(
                new SelectiveOriginEntityProviderLogic(ids), 
                _originEntitySource,
                _logicOptions.PageSize
                );

            var entityIndexer = new EntityIndexer(readyIndexProvider, entityProvider);

            await entityIndexer.IndexAsync();
        }

        private async Task DeindexAsync(string[] ids)
        {
            var deindexerProvider = new ExistentDeindexerProvider(_esOptions.IndexName, _indexManager);
            var deindexer = await deindexerProvider.ProvideAsync();
            await deindexer.DeindexAsync(ids);
        }

        private async Task ReindexAsync()
        {
            await using var reindexer = await ReindexOperator.StartReindexingAsync(_esOptions.IndexName, _indexManager);

            var entityProvider = new OriginEntityProvider(
                new AllOriginEntityProviderLogic(),
                _originEntitySource,
                _logicOptions.PageSize
            );
            var entityIndexer = new EntityIndexer(reindexer, entityProvider);

            await entityIndexer.IndexAsync();
            await reindexer.CommitAsync();
        }
    }
}
