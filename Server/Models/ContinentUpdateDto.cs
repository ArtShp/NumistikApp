namespace Server.Models;

public static class ContinentUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        [ContinentName]
        public required string Name { get; set; }
    }
}
