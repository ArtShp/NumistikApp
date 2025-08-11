using Server.Models.Extensions;

namespace Server.Models.CollectionItemSpecialStatus;

public static class CollectionItemSpecialStatusUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        [NameWithSpacesNoNumbers]
        public required string Name { get; set; }
    }
}
