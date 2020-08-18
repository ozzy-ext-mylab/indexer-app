using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;

namespace MyLab.Indexer.Tools
{
    class SelectiveOriginEntityProvider : IOriginEntityProvider
    {
        private readonly string[] _ids;

        public SelectiveOriginEntityProvider(string[] ids)
        {
            _ids = ids ?? throw new ArgumentNullException(nameof(ids));
        }

        public IAsyncEnumerable<OriginEntity[]> Provide(IOriginEntitySource src, int pageSize)
        {
            return new SelectiveOriginEntityEnumerable(_ids, src, pageSize);
        }

        class SelectiveOriginEntityEnumerable : IAsyncEnumerable<OriginEntity[]>
        {
            private readonly string[] _ids;
            private readonly IOriginEntitySource _src;
            private readonly int _pageSize;

            public SelectiveOriginEntityEnumerable(string[] ids, IOriginEntitySource src, in int pageSize)
            {
                _ids = ids;
                _src = src;
                _pageSize = pageSize;
            }

            public IAsyncEnumerator<OriginEntity[]> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
            {
                return new SelectiveOriginEntityEnumerator(_ids, _src, _pageSize, cancellationToken);
            }
        }

        class SelectiveOriginEntityEnumerator : IAsyncEnumerator<OriginEntity[]>
        {
            private readonly string[] _ids;
            private readonly IOriginEntitySource _src;
            private readonly int _pageSize;
            private readonly CancellationToken _cancellationToken;

            private int _pageIndex;

            public OriginEntity[] Current { get; set; }

            public SelectiveOriginEntityEnumerator(
                string[] ids,
                IOriginEntitySource src, 
                int pageSize, 
                CancellationToken cancellationToken)
            {
                _ids = ids;
                _src = src;
                _pageSize = pageSize;
                _cancellationToken = cancellationToken;
            }

            public ValueTask DisposeAsync()
            {
                _pageIndex = 0;

                return new ValueTask(Task.CompletedTask);
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                var pageIds = _ids
                    .Skip(_pageIndex * _pageSize)
                    .Take(_pageSize)
                    .ToArray();

                if (pageIds.Length == 0) 
                    return false;

                var q = _src.Query()
                    .Where(itm => pageIds.Contains(itm.Id));

                Current = await q.ToArrayAsync(_cancellationToken);

                _pageIndex++;

                return Current.Length != 0;
            }
        }

    }
}
