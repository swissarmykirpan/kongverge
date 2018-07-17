using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Validation.DTOs;

namespace Kongverge.Validation.Helpers
{
    public class TestTracker
    {
        public TestTracker()
        {
            TestResultTasks = new ConcurrentDictionary<string, TaskCompletionSource<Test>>();
            TestList = new ConcurrentDictionary<string, Test>();
        }

        public IDictionary<string, TaskCompletionSource<Test>> TestResultTasks { get; set; }
        public IDictionary<string, Test> TestList { get; set; }
    }
}
