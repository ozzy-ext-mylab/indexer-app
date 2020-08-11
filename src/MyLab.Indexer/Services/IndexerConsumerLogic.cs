using System;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Mq;

namespace MyLab.Indexer.Services
{
    class IndexerConsumerLogic : IMqConsumerLogic<IndexingMsg>
    {
        private readonly OriginEntityProvider _entityProvider;
        private readonly IEntityIndexManager _indexManager;
        private readonly IReporter _reporter;

        /// <summary>
        /// Initializes a new instance of <see cref="IndexerConsumerLogic"/>
        /// </summary>
        public IndexerConsumerLogic(
            OriginEntityProvider entityProvider,
            IEntityIndexManager indexManager,
            IReporter reporter = null)
        {
            _entityProvider = entityProvider ?? throw new ArgumentNullException(nameof(entityProvider));
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));
            _reporter = reporter;
        }

        public async Task Consume(MqMessage<IndexingMsg> message)
        {
            if (message.Payload.Reindex)
            {
                await Reindex();
            }
            if(message.Payload.Update != null && message.Payload.Update.Length > 0)
                await Update(message.Payload.Update);

            var dList = message.Payload.Delete;
            if (dList != null && dList.Length > 0)
                await Delete(dList);
        }

        private async Task Delete(string[] dList)
        {
            await _indexManager.RemoveEntitiesAsync(dList);
        }

        private async Task Update(string[] updateList)
        {
            var lost = updateList?.ToList();

            await foreach (var entityBatch in _entityProvider.ProvideEntities(updateList))
            {
                var docs = entityBatch.ToArray();

                await _indexManager.IndexEntityBatchAsync(docs);

                lost?.RemoveAll(id => entityBatch.Any(e => e.Id == id));
            }

            if (lost != null && lost.Count > 0)
                _reporter?.ReportAboutLostEntities(lost.ToArray());
        }

        private async Task Reindex()
        {
            //await _indexManager.StartReindexAsync();

            //await foreach (var entityBatch in _entityProvider.ProvideAllEntities())
            //{
            //    var docs = entityBatch.ToArray();

            //    await _indexManager.IndexEntityBatchAsync(docs);
            //}

            //await _indexManager.EndReindexAsync();

            throw new NotImplementedException();
        }
    }
}