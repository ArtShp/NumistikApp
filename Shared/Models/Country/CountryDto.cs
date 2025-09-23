namespace Shared.Models.Country;

public static class CountryDto
{
    public class Response
    {
        public required int Id { get; set; }

        public required string Name { get; set; }

        public required int ContinentId { get; set; }
    }
}
