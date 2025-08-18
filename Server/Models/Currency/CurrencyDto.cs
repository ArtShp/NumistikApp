namespace Server.Models.Currency;

public static class CurrencyDto
{
    public class Response
    {
        public required int Id { get; set; }

        public required string MajorName { get; set; }

        public required string MinorName { get; set; }

        public required string Code { get; set; }

        public required string Symbol { get; set; }
    }
}
