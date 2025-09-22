using Shared.Models.Extensions;

namespace Shared.Models.CollectionItemStatus;

public static class CollectionItemStatusUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        [NameWithSpacesNoNumbers]
        public required string Name { get; set; }
    }
}
