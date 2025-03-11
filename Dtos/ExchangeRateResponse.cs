public class ExchangeRateResponse
{
    public bool Success { get; set; }
    public int Timestamp { get; set; }
    public Dictionary<string, decimal>? Quotes { get; set; } 
}