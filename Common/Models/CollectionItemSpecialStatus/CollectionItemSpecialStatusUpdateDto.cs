using Shared.Models.Extensions;

namespace Shared.Models.CollectionItemSpecialStatus;

public static class CollectionItemSpecialStatusUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        [NameWithSpacesNoNumbers]
        public required string Name { get; set; }
    }
}
