namespace ApexPredatorTrialsAPI.DTOs
{
    public class GameEventRegistrationDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int PlayerId { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
    public class GameEventRegistrationCreateDto
    {
        public int EventId { get; set; }
        public int PlayerId { get; set; }
    }
}
