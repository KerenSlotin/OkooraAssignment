using System.Text.Json;

public class FileHandler : IDataStoreHandler
{
  private string _filePath;
  
  public FileHandler(string filePath)
  {
    _filePath = filePath;
  }

  public async Task<Dictionary<CurrencyPair, ExchangeRateData>> ReadRatesFromStorageAsync()
  {
    if(File.Exists(_filePath))
    {
      var fileContent = await File.ReadAllTextAsync(_filePath);
      var res = JsonSerializer.Deserialize<Dictionary<string, ExchangeRateData>>(fileContent) ?? new ();
      return res.ToDictionary(kvp => new CurrencyPair(kvp.Key), kvp => kvp.Value);
    }
    return new();
  }

  public async Task WriteRateToStorageAsync(Dictionary<CurrencyPair, ExchangeRateData> rates)
  {
    var ratesToWrite = rates.ToDictionary(kvp => kvp.Key.FromTo, kvp => kvp.Value);
    var jsonContent = JsonSerializer.Serialize(ratesToWrite);
    await File.WriteAllTextAsync(_filePath, jsonContent);
  }
}