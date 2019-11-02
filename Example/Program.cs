using System.Collections.Generic;
using Aljurythm;

namespace Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            var jury = new Jury<int>
            {
                Name = "Sum Algorithm",
                Levels = new List<Level>
                {
                    new Level
                    {
                        Name = "Sample Cases",
                        Path = @"Tests/sample.txt",
                        TimeLimit = 35,
                        RunMultiplier = 1E4,
                        DisplayInputs = true,
                        InputSeparator = ", "
                    },
                    new Level
                    {
                        Name = "Complete Cases",
                        Path = @"Tests/complete.txt",
                        TimeLimit = 8,
                        DisplayLog = false
                    }
                },
                // Specify how to read a single test case (input and expected result)
                // from the Level's file, using streamReader to read the file.
                ReadInput = (testCase, streamReader) =>
                {
                    var operands = streamReader.ReadLine().Split(' ');
                    testCase["x"] = int.Parse(operands[0]);
                    testCase["y"] = int.Parse(operands[1]);
                    testCase.Expected = int.Parse(streamReader.ReadLine());
                },
                // Test the required Algorithms using the inputs as follow:
                // (The algorithm is run as many times as Level.RunMultiplier)
                Algorithm = testCase => RunAlgorithm((int)testCase["x"], (int)testCase["y"])
            };

            jury.DisplayMenu();
            jury.Start();

        }

        private static int RunAlgorithm(int x, int y) => x + y;
    }
}
