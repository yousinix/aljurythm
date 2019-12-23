using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Aljurythm
{
    public class TestCase
    {
        private readonly Level _level;
        private readonly int _index;
        private readonly string _format;

        public TestCase(Level level, int index, string format)
        {
            _level = level;
            _index = index;
            _format = format;
        }

        public string Number => (_index + 1).ToString(_format);
        public Dictionary<string, object> Input { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> Expected { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> Actual { get; } = new Dictionary<string, object>();

        public string InputsLog()
        {
            var log = Input.Select(p => $"{p.Key} = {p.Value}").ToArray();
            return string.Join(_level.InputSeparator, log);
        }

        public string FailureLog(TestResult result)
        {
            var log = result.FailedKeys.Select(key => $"[ {key} => you: {Actual[key]} | jury: {Expected[key]} ]");
            return string.Join("\n", log);
        }

        public TestResult Evaluate(Action<TestCase> algorithm)
        {
            return new TestResult
            {
                TimeLimit = _level.TimeLimit,
                ElapsedTime = CalculateTime(() => RunMultiplied(algorithm)),
                FailedKeys = Expected.Where(e => !e.Value.Equals(Actual[e.Key])).Select(x => x.Key).ToList()
            };
        }

        private void RunMultiplied(Action<TestCase> algorithm)
        {
            for (var i = 0; i < _level.RunMultiplier; i++)
            {
                algorithm(this);
            }
        }

        private static long CalculateTime(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}