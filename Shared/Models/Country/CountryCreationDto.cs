using Shared.Models.Extensions;

namespace Shared.Models.Country;

public static class CountryCreationDto
{
    public class Request
    {
        [NameWithSpacesNoNumbers]
        public required string Name { get; set; }

        public required int ContinentId { get; set; }
    }

    public class Response
    {
        public required int Id { get; set; }

        public required string Name { get; set; }
    }
}
