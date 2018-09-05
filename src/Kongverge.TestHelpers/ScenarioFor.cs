using Moq.AutoMock;

namespace Kongverge.TestHelpers
{
    public abstract class ScenarioFor<T> : AutoMocker where T : class
    {
        private T _subject;
        public T Subject => _subject ?? (_subject = CreateInstance<T>());
    }
}
