namespace Aljurythm
{
    public class Level
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long TimeLimit { get; set; } = long.MaxValue;
        public double RunMultiplier { get; set; } = 1;
        public bool DisplayInputs { get; set; }
        public string InputSeparator { get; set; } = "\n";
        public bool DisplayLog { get; set; } = true;
        internal Statistics Statistics { get; } = new Statistics();
    }
}