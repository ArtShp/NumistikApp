using Shared.Models.Extensions;

namespace Shared.Models.CollectionItemQuality;

public static class CollectionItemQualityUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        [NameWithSpacesNoNumbers]
        public required string Name { get; set; }
    }
}
