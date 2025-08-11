using Server.Models.Extensions;

namespace Server.Models.CollectionItemSpecialStatus;

public static class CollectionItemSpecialStatusCreationDto
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
