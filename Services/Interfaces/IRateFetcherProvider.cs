

public interface IRateFetcherProvider
{
    public Dictionary<CurrencyPair, ExchangeRateData> GetRates();
    public ExchangeRateData GetSpecificRate(string fromCurrency, string toCurrency);
}