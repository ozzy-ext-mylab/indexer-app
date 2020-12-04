using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Indexer.Common;
using MyLab.LogDsl;
using MyLab.Mq;
using MyLab.Mq.PubSub;

namespace MyLab.Indexer
{
    class IndexerConsumerLogic : IMqConsumerLogic<IndexingMsg>
    {
        private readonly IOriginEntitySource _originEntitySource;
        private readonly IIndexManager _indexManager;
        private readonly IndexerOptions _options;
        private readonly DslLogger _log;

        public IndexerConsumerLogic(
            IOriginEntitySource originEntitySource,
            IIndexManager indexManager,
            IndexerOptions options,
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
            IOptions<IndexerOptions> options,
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

            if (message.Payload.DeindexList != null)
                await DeindexAsync(message.Payload.DeindexList);
            if (message.Payload.IndexList != null)
                await IndexAsync(message.Payload.IndexList);
        }

        private async Task IndexAsync(string[] ids)
        {
            var readyIndexProvider = new ReadyIndexProvider(_options.IndexName, _indexManager);
            
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
            var deindexerProvider = new ExistentDeindexerProvider(_options.IndexName, _indexManager);
            var deindexer = await deindexerProvider.ProvideAsync();
            await deindexer.DeindexAsync(ids);
        }
    }
}
