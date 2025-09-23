using Shared.Models.Extensions;

namespace Shared.Models.CollectionItemType;

public static class CollectionItemTypeUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        [NameWithSpacesNoNumbers]
        public required string Name { get; set; }
    }
}
