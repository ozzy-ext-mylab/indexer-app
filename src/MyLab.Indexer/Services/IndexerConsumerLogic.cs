using System;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Mq;

namespace MyLab.Indexer.Services
{
    class IndexerConsumerLogic : IMqConsumerLogic<IndexingMsg>
    {
        private readonly OriginEntityProvider _entityProvider;
        private readonly IEntityIndexer _indexer;
        private readonly IReporter _reporter;

        /// <summary>
        /// Initializes a new instance of <see cref="IndexerConsumerLogic"/>
        /// </summary>
        public IndexerConsumerLogic(
            OriginEntityProvider entityProvider,
            IEntityIndexer indexer,
            IReporter reporter)
        {
            _entityProvider = entityProvider ?? throw new ArgumentNullException(nameof(entityProvider));
            _indexer = indexer ?? throw new ArgumentNullException(nameof(indexer));
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
        }

        public async Task Consume(MqMessage<IndexingMsg> message)
        {
            await Update(message.Payload.Update);

            var dList = message.Payload.Delete;
            if (dList != null && dList.Length > 0)
                await Delete(dList);
        }

        private async Task Delete(string[] dList)
        {
            await _indexer.RemoveEntitiesAsync(dList);
        }

        private async Task Update(string[] updateList)
        {
            var lost = updateList.ToList();

            await foreach (var entityBatch in _entityProvider.ProvideEntities(updateList))
            {
                var docs = entityBatch.ToArray();

                await _indexer.IndexEntityBatchAsync(docs);

                lost.RemoveAll(id => entityBatch.Any(e => e.Id == id));
            }

            if (lost.Count > 0)
                _reporter.ReportAboutLostEntities(lost.ToArray());
        }
    }
}