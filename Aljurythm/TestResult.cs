using System.Collections.Generic;

namespace Aljurythm
{
    public class TestResult
    {
        public long ElapsedTime { get; set; }
        public long TimeLimit { get; set; }
        public bool HasExceededTimeLimit => ElapsedTime > TimeLimit;
        public bool HasFailed => FailedKeys.Count != 0;
        public List<string> FailedKeys { get; set; }
    }
}
