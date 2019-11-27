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
        public bool HasFailed => Failed.Count != 0;


        public Dictionary<string, object> Input { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Expected { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Actual { get; set; } = new Dictionary<string, object>();
        public List<string> Failed { get; set; } = new List<string>();


        public string InputsLog
        {
            get
            {
                var array = Input.Select(p => $"{p.Key} = {p.Value}").ToArray();
                return string.Join(_level.InputSeparator, array);
            }
        }

        public string FailureLog
        {
            get
            {
                var result = Failed.Select(key => $"[ {key} => you: {Actual[key]} | jury: {Expected[key]} ]");
                return string.Join("\n", result);
            }
        }

        public void Run(Action<TestCase> algorithm)
        {
            Time = CalculateTime(() =>
            {
                for (var i = 0; i < _level.RunMultiplier; i++) algorithm(this);
            });
        }

        public void Evaluate()
        {
            foreach (var output in Expected.Where(output => !output.Value.Equals(Actual[output.Key])))
                Failed.Add(output.Key);
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