using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Aljurythm
{
    public class TestCase
    {
        private readonly int _index;
        private readonly Level _level;
        private readonly int _padding;

        public TestCase(Level level, int index, int padding)
        {
            _level = level;
            _index = index;
            _padding = padding;
        }

        public string Number => (_index + 1).ToString().PadLeft(_padding, '0');
        public long Time { get; set; }
        public bool HasExceededTimeLimit => Time > _level.TimeLimit;
        public bool HasFailed => !string.IsNullOrEmpty(FailureLog);


        public Dictionary<string, object> Inputs { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> ExpectedOutputs { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> ActualOutputs { get; set; } = new Dictionary<string, object>();


        public string InputsLog
        {
            get
            {
                var array = Inputs.Select(p => $"{p.Key} = {p.Value}").ToArray();
                return string.Join(_level.InputSeparator, array);
            }
        }

        public string FailureLog { get; private set; }


        public void Test(Action<TestCase> algorithm)
        {
            Time = CalculateTime(() =>
            {
                for (var i = 0; i < _level.RunMultiplier; i++) algorithm(this);
            });
            CompareResults();
        }

        private static long CalculateTime(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private void CompareResults()
        {
            foreach (var output in ExpectedOutputs.Where(output => !output.Value.Equals(ActualOutputs[output.Key])))
                AddToLog(output.Key);
        }

        private void AddToLog(string key)
        {
            FailureLog += $"[ {key} => " +
                          $"you: {ActualOutputs[key]} | " +
                          $"jury: {ExpectedOutputs[key]} ]\n";
        }
    }
}