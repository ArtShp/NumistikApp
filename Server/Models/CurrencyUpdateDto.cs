namespace Server.Models;

public static class CurrencyUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        [NameWithSpacesNoNumbers]
        public string? MajorName { get; set; }

        [NameWithSpacesNoNumbers]
        public string? MinorName { get; set; }

        [CurrencyCode]
        public string? Code { get; set; }

        [CurrencySymbol]
        public string? Symbol { get; set; }
    }
}
