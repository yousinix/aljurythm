using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Aljurythm
{
    public class TestCase<TResult> where TResult : struct, IEquatable<TResult>
    {
        private readonly string _inputSeparator;

        private Stopwatch _stopwatch;

        public TestCase(string inputSeparator)
        {
            _inputSeparator = inputSeparator;
        }

        public TResult Expected { get; set; }
        public TResult Actual { get; set; }
        public long Time { get; set; }

        public Dictionary<string, object> Inputs { get; set; } = new Dictionary<string, object>();

        public object this[string key]
        {
            get => Inputs[key];
            set => Inputs[key] = value;
        }

        public void RunAlgorithm(Func<TResult> algorithm, double multiplierFactor)
        {
            _stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < multiplierFactor; i++)
            {
                Actual = algorithm();
            }
            _stopwatch.Stop();
            Time = _stopwatch.ElapsedMilliseconds;
        }

        public void PrintInput()
        {
            var array = Inputs.Select(p => $"{p.Key} = {p.Value}").ToArray();
            Console.WriteLine(string.Join(_inputSeparator, array) + "\n");
        }
    }
}