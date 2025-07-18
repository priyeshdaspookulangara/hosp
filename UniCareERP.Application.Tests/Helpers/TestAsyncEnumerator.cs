using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniCareERP.Application.Tests.Helpers
{
    // Helper class for mocking IAsyncEnumerable for EF Core operations like ToListAsync, FirstOrDefaultAsync
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;
        public T Current => _enumerator.Current;
        public TestAsyncEnumerator(IEnumerator<T> enumerator) => _enumerator = enumerator;
        public ValueTask DisposeAsync() => new ValueTask(Task.CompletedTask);
        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_enumerator.MoveNext());
    }
}
