namespace Aljurythm
{
    public class Level
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long TimeLimit { get; set; } = long.MaxValue;
        public double MultiplierFactor { get; set; } = 1;
        public bool DisplayInputs { get; set; } = false;
        public bool DisplayLog { get; set; } = true;
        internal Statistics Statistics { get; set; } = new Statistics();
    }
}