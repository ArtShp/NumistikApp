using Shared.Models.Extensions;

namespace Shared.Models.Continent;

public static class ContinentUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        [NameWithSpacesNoNumbers]
        public required string Name { get; set; }
    }
}
