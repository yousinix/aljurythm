using System;

namespace Aljurythm
{
    internal class Statistics
    {
        public int TotalCases { get; set; }
        public int FailedCases { get; set; } = 0;
        public double CompletedCases => TotalCases - FailedCases;
        public double Percentage => CompletedCases / TotalCases * 100;

        public double TotalTime { get; set; } = 0;
        public double MaxTime { get; set; } = -1;
        public double AvgTime => TotalTime / TotalCases;

        public void UpdateTime<T>(TestCase<T> testCase) where T : struct, IEquatable<T>
        {
            TotalTime += testCase.Time;
            MaxTime = MaxTime > testCase.Time ? MaxTime : testCase.Time;
        }

        public void Print()
        {
            const int col1Width = 18;
            const int col2Width = 10;
            const int col1Alignment = -col1Width + 2;
            const int col2Alignment = -col2Width + 2;
            var col1Separator = new string('─', col1Width);
            var col2Separator = new string('─', col2Width);

            Console.WriteLine();
            Console.WriteLine("┌" + col1Separator + "┬" + col2Separator + "┐");

            Console.WriteLine($"│ {"Total Cases",col1Alignment} │ {TotalCases,col2Alignment} │");
            Console.WriteLine($"│ {"Failed",col1Alignment} │ {FailedCases,col2Alignment} │");
            Console.WriteLine($"│ {"Completed",col1Alignment} │ {CompletedCases,col2Alignment} │");
            Console.WriteLine($"│ {"Percentage",col1Alignment} │ {AddSuffix(Percentage, "%"),col2Alignment} │");

            Console.WriteLine("├" + col1Separator + "┼" + col2Separator + "┤");

            Console.WriteLine($"│ {"Total Time",col1Alignment} │ {AddSuffix(TotalTime, " ms"),col2Alignment} │");
            Console.WriteLine($"│ {"Maximum",col1Alignment} │ {AddSuffix(MaxTime, " ms"),col2Alignment} │");
            Console.WriteLine($"│ {"Average",col1Alignment} │ {AddSuffix(AvgTime, " ms"),col2Alignment} │");

            Console.WriteLine("└" + col1Separator + "┴" + col2Separator + "┘");
            Console.WriteLine();
        }

        private static string AddSuffix(dynamic val, string suffix) => $"{val}{suffix}";
    }
}