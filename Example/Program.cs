using System.Collections.Generic;
using Aljurythm;

namespace Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            var jury = new Jury
            {
                Name = "Sum Algorithm",
                Levels = new List<Level>
                {
                    new Level
                    {
                        Name = "Sample Cases",
                        Path = @"Tests/sample.txt",
                        TimeLimit = 35,
                        MultiplierFactor = 1E4,
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
                }
            };

            jury.DisplayMenu();
            jury.Evaluate((level, streamReader) =>
            {
                // Create an instance from TestCase using the type of the result.
                var testCase = new TestCase<int>(level.InputSeparator);

                var operands = streamReader.ReadLine().Split(' ');
                testCase["x"] = int.Parse(operands[0]);
                testCase["y"] = int.Parse(operands[1]);
                testCase.Expected = int.Parse(streamReader.ReadLine());

                // Test the required Algorithms using the inputs as follow:
                // P.S: You can multiply the runtime to make sure the order is as required.
                testCase.Test(() => RunAlgorithm((int)testCase["x"], (int)testCase["y"]));

                return testCase;
            });

        }

        private static int RunAlgorithm(int x, int y) => x + y;
    }
}
