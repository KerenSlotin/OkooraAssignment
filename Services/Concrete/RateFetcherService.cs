using System.Diagnostics;
using System.Text.Json;

public class RateFetcherService : IHostedService, IRateFetcherProvider, IAsyncDisposable
{
    private Timer? _timer;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private Dictionary<CurrencyPair, ExchangeRateData> _ratesForCurrencies;
    private readonly IDataStoreHandler _dataManager;

    public RateFetcherService(IDataStoreHandler dataManager)
    {
        _apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? throw new ArgumentNullException("Provide an environment variable for API_KEY");
        _httpClient = new();
        _dataManager = dataManager;
        _ratesForCurrencies = _dataManager.ReadRatesFromStorageAsync().GetAwaiter().GetResult();
        InitRatesForCurrencyPairs();
    }

    private void InitRatesForCurrencyPairs()
    {
        if (_ratesForCurrencies.Count == 0)
        {
            var usdIls = new CurrencyPair(Currency.USD, Currency.ILS);
            _ratesForCurrencies[usdIls] = new ExchangeRateData() { CurrencyPair = usdIls.FromTo };
            var eurIls = new CurrencyPair(Currency.EUR, Currency.ILS);
            _ratesForCurrencies[eurIls] = new ExchangeRateData() { CurrencyPair = eurIls.FromTo };
            var gbpIls = new CurrencyPair(Currency.GBP, Currency.ILS);
            _ratesForCurrencies[new(Currency.GBP, Currency.ILS)] = new ExchangeRateData() { CurrencyPair = gbpIls.FromTo };
            var eurUsd = new CurrencyPair(Currency.EUR, Currency.USD);
            _ratesForCurrencies[new(Currency.EUR, Currency.USD)] = new ExchangeRateData() { CurrencyPair = eurUsd.FromTo };
            var eurGbp = new CurrencyPair(Currency.EUR, Currency.GBP);
            _ratesForCurrencies[new(Currency.EUR, Currency.GBP)] = new ExchangeRateData() { CurrencyPair = eurGbp.FromTo };
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
         // unfortunately the rate limit of the API doesn't allow so many requests so I had to change the frequency from 10 seconds to 10 minutes
        _timer = new Timer(async _ => await FetchExchangeRates(), null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
        return Task.CompletedTask;
    }
    

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_timer is IAsyncDisposable timer)
        {
            await timer.DisposeAsync();
        }

        _timer = null;
    }

    private async Task FetchExchangeRates()
    {
        foreach(var currencyToRate in _ratesForCurrencies)
        {
            var fromCurrency = currencyToRate.Key.From;
            var toCurrency = currencyToRate.Key.To;
            Console.WriteLine(fromCurrency);
            Console.WriteLine(toCurrency);
            var requestUrl = $"https://api.currencylayer.com/live?access_key={_apiKey}&source={fromCurrency}&currencies={toCurrency}";
            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var exchangeRateResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ExchangeRateResponse>(jsonResponse);
                _ratesForCurrencies[currencyToRate.Key].LastUpdateTime = DateTime.UtcNow;
                Console.WriteLine(jsonResponse);
                _ratesForCurrencies[currencyToRate.Key].Rate = exchangeRateResponse!.Quotes![$"{fromCurrency}{toCurrency}"];
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Error fetching exchange rates: {ex.Message}");
            }
        }
        await _dataManager.WriteRateToStorageAsync(_ratesForCurrencies);
    }

    public Dictionary<CurrencyPair, ExchangeRateData> GetRates()
    {
        return _ratesForCurrencies;
    }

    public ExchangeRateData GetSpecificRate(string fromCurrency, string toCurrency)
    {
        if (!Enum.TryParse(fromCurrency.ToUpper(), out Currency fromCurrencyEnum))
        {
            throw new ArgumentException($"Invalid currency: {fromCurrency}");
        }

        if (!Enum.TryParse(toCurrency.ToUpper(), out Currency toCurrencyEnum))
        {
            throw new ArgumentException($"Invalid currency: {toCurrency}");
        }

        var currencyPair = new CurrencyPair(fromCurrencyEnum, toCurrencyEnum);
        return _ratesForCurrencies.TryGetValue(currencyPair, out var rateData) ? rateData : throw new ArgumentException("Rates for these currencies are not being tracked");
    }
}