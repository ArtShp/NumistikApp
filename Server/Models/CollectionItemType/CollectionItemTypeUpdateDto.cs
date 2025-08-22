using Server.Models.Extensions;

namespace Server.Models.CollectionItemType;

public static class CollectionItemTypeUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        [NameWithSpacesNoNumbers]
        public required string Name { get; set; }
    }
}
