using Shared.Models.Common;

namespace Shared.Models.Collection;

public static class CollectionUpdateRoleDto
{
    public class Request
    {
        public required Guid CollectionId { get; set; }

        public required Guid UserId { get; set; }

        public required CollectionRole Role { get; set; }
    }
}
