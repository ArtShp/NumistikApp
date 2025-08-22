using Server.Models.Extensions;

namespace Server.Models.CollectionItemQuality;

public static class CollectionItemQualityCreationDto
{
    public class Request
    {
        [NameWithSpacesNoNumbers]
        public required string Name { get; set; }
    }

    public class Response
    {
        public required int Id { get; set; }

        public required string Name { get; set; }
    }
}
