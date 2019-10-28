namespace Aljurythm
{
    public class Level
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long TimeLimit { get; set; }
        public double MultiplierFactor { get; set; } = 1;
    }
}