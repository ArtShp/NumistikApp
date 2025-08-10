namespace Server.Models;

public static class CurrencyCreationDto
{
    public class Request
    {
        [NameWithSpacesNoNumbers]
        public required string MajorName { get; set; }

        [NameWithSpacesNoNumbers]
        public required string MinorName { get; set; }

        [CurrencyCode]
        public required string Code { get; set; }

        [CurrencySymbol]
        public required string Symbol { get; set; }
    }

    public class Response
    {
        public required int Id { get; set; }

        public required string MajorName { get; set; }

        public required string Code { get; set; }
    }
}
