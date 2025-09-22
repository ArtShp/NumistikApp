namespace Shared.Models.Collection;

public static class CollectionUpdateDto
{
    public class Request
    {
        public required Guid Id { get; set; }

        public required string? Name { get; set; }

        public required string? Description { get; set; }
    }
}
