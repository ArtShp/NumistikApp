using Shared.Models.Common;
using Shared.Models.Extensions;

namespace Shared.Models.Collection;

public static class CollectionUpdateRoleDto
{
    public class Request
    {
        public required Guid CollectionId { get; set; }

        public Guid? UserId { get; set; }

        [Username]
        public string? Username { get; set; }

        public required CollectionRole Role { get; set; }
    }
}
