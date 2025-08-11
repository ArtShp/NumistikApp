using Server.Entities;

namespace Server.Models.Collection;

public static class CollectionAssignDto
{
    public class Request
    {
        public required Guid CollectionId { get; set; }

        public required Guid UserId { get; set; }

        public required CollectionRole Role { get; set; }
    }
}
