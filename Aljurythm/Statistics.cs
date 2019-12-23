using System;

namespace Aljurythm
{
    internal class Statistics
    {
        private int TotalCases { get; set; }
        private int FailedCases { get; set; }
        private double CompletedCases => TotalCases - FailedCases;
        private double Percentage => Math.Round(CompletedCases / TotalCases * 100, 2);
        private double TotalTime { get; set; }
        private double MaxTime { get; set; } = -1;
        private double AvgTime => Math.Round(TotalTime / TotalCases, 2);

        public void Update(TestResult testResult)
        {
            TotalCases++;
            if (testResult.HasFailed) FailedCases++;
            TotalTime += testResult.ElapsedTime;
            MaxTime = MaxTime > testResult.ElapsedTime ? MaxTime : testResult.ElapsedTime;
        }

        public void Print()
        {
            const int col1Width = 18;
            const int col2Width = 14;
            const int col1Alignment = -col1Width + 2;
            const int col2Alignment = -col2Width + 2;
            var col1Separator = new string('─', col1Width);
            var col2Separator = new string('─', col2Width);

            Logger.LineBreak();
            Logger.WriteLine("┌" + col1Separator + "┬" + col2Separator + "┐");

            Logger.WriteLine($"│ {"Total Cases",col1Alignment} │ {TotalCases,col2Alignment} │");
            Logger.WriteLine($"│ {"Failed",col1Alignment} │ {FailedCases,col2Alignment} │");
            Logger.WriteLine($"│ {"Completed",col1Alignment} │ {CompletedCases,col2Alignment} │");
            Logger.WriteLine($"│ {"Percentage",col1Alignment} │ {AddSuffix(Percentage, "%"),col2Alignment} │");

            Logger.WriteLine("├" + col1Separator + "┼" + col2Separator + "┤");

            Logger.WriteLine($"│ {"Total Time",col1Alignment} │ {AddSuffix(TotalTime, " ms"),col2Alignment} │");
            Logger.WriteLine($"│ {"Maximum",col1Alignment} │ {AddSuffix(MaxTime, " ms"),col2Alignment} │");
            Logger.WriteLine($"│ {"Average",col1Alignment} │ {AddSuffix(AvgTime, " ms"),col2Alignment} │");

            Logger.WriteLine("└" + col1Separator + "┴" + col2Separator + "┘");
            Logger.LineBreak();
        }

        private static string AddSuffix(dynamic val, string suffix)
        {
            return $"{val}{suffix}";
        }
    }
}