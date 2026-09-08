namespace ApexPredatorTrialsAPI.Models
{
    public class Log
    {
        public int Id { get; set; }
        public required string? ClientAddress { get; set; }
        public required string Path { get; set; }
        public required string Method { get; set; }
        public double Duration { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
