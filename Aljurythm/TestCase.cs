using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Aljurythm
{
    public class TestCase<TResult> where TResult : struct, IEquatable<TResult>
    {
        private Stopwatch _stopwatch;
        public TResult Expected { get; set; }
        public TResult Actual { get; set; }
        public long Time { get; set; }
        public InputsList Inputs { get; set; } = new InputsList();

        public void Test(Func<TResult> algorithm, double multiplierFactor = 1)
        {
            _stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < multiplierFactor; i++)
            {
                Actual = algorithm();
            }
            _stopwatch.Stop();
            Time = _stopwatch.ElapsedMilliseconds;
        }
    }
}