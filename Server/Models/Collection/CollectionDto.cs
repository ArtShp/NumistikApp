using Server.Entities;

namespace Server.Models.Collection;

public static class CollectionDto
{
    public class Response
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required string? Description { get; set; }

        public required CollectionRole CollectionRole { get; set; }
    }
}
