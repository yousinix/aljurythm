using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Aljurythm
{
    public class TestCase<TResult> where TResult : struct, IEquatable<TResult>
    {
        private readonly Level _level;
        private readonly int _index;
        private readonly int _padding;
        private Stopwatch _stopwatch;

        public TestCase(Level level, int index, int padding)
        {
            _level = level;
            _index = index;
            _padding = padding;
        }

        public string Number => (_index + 1).ToString().PadLeft(_padding, '0');
        public TResult Expected { get; set; }
        public TResult Actual { get; set; }
        public long Time { get; set; }
        public bool HasFailed { get; set; }
        public bool HasExceededTimeLimit { get; set; }
        public Dictionary<string, object> Inputs { get; set; } = new Dictionary<string, object>();

        public object this[string key]
        {
            get => Inputs[key];
            set => Inputs[key] = value;
        }

        public void RunAlgorithm(Func<TResult> algorithm)
        {
            _stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < _level.RunMultiplier; i++) Actual = algorithm();
            _stopwatch.Stop();
            Time = _stopwatch.ElapsedMilliseconds;
            HasFailed = !Actual.Equals(Expected);
            HasExceededTimeLimit = Time > _level.TimeLimit;
        }

        public void PrintInput()
        {
            var array = Inputs.Select(p => $"{p.Key} = {p.Value}").ToArray();
            Logger.WriteLine(string.Join(_level.InputSeparator, array));
            Logger.LineBreak();
        }
    }
}