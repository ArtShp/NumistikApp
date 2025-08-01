namespace Server.Models;

public static class CollectionCreationDto
{
    public class Request
    {
        public required string Name { get; set; }

        public string? Description { get; set; }
    }

    public class Response
    {
        public required Guid Id { get; set; }
    }
}
