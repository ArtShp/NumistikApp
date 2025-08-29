using Shared.Models.Extensions;

namespace Shared.Models.Country;

public static class CountryUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        [NameWithSpacesNoNumbers]
        public string? Name { get; set; }

        public int? ContinentId { get; set; }
    }
}
