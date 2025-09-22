using Shared.Models.Common;

namespace Shared.Models.Collection;

public static class CollectionMembersDto
{
    public class Member
    {
        public required Guid UserId { get; set; }

        public required string Username { get; set; }

        public required CollectionRole Role { get; set; }
    }

    public class Response
    {
        public required Guid CollectionId { get; set; }

        public required List<Member> Members { get; set; }
    }
}
