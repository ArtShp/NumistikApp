using Server.Models.Extensions;

namespace Server.Models.Continent;

public static class ContinentUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        [NameWithSpacesNoNumbers]
        public required string Name { get; set; }
    }
}
