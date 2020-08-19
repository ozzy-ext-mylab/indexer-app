using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;

namespace MyLab.Indexer.Tools
{
    class AllOriginEntityProviderLogic : IOriginEntityProviderLogic
    {
        public IAsyncEnumerable<OriginEntity[]> Provide(IOriginEntitySource src, int pageSize)
        {
            return new AllOriginEntityEnumerable(src, pageSize);
        }

        class AllOriginEntityEnumerable : IAsyncEnumerable<OriginEntity[]>
        {
            private readonly IOriginEntitySource _src;
            private readonly int _pageSize;

            public AllOriginEntityEnumerable(IOriginEntitySource src, in int pageSize)
            {
                _src = src;
                _pageSize = pageSize;
            }

            public IAsyncEnumerator<OriginEntity[]> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
            {
                return new AllOriginEntityEnumerator(_src, _pageSize, cancellationToken);
            }
        }

        class AllOriginEntityEnumerator : IAsyncEnumerator<OriginEntity[]>
        {
            private readonly IOriginEntitySource _src;
            private readonly int _pageSize;
            private readonly CancellationToken _cancellationToken;

            private int _pageIndex;

            public OriginEntity[] Current { get; set; }

            public AllOriginEntityEnumerator(IOriginEntitySource src, int pageSize, CancellationToken cancellationToken)
            {
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
                var q = _src.Query();

                    q = q
                        .Skip(_pageIndex * _pageSize)
                        .Take(_pageSize);

                _pageIndex++;

                Current = await q.ToArrayAsync(_cancellationToken);

                return Current.Length != 0;
            }
        }
    }
}