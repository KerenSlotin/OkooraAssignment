public interface IDataStoreHandler
{
  public Task<Dictionary<CurrencyPair, ExchangeRateData>> ReadRatesFromStorageAsync();
  public Task WriteRateToStorageAsync(Dictionary<CurrencyPair, ExchangeRateData> rates);
}