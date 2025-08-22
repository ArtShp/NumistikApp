using Server.Models.Extensions;

namespace Server.Models.CollectionItemQuality;

public static class CollectionItemQualityUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        [NameWithSpacesNoNumbers]
        public required string Name { get; set; }
    }
}
