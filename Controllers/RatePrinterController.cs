using Microsoft.AspNetCore.Mvc;

[Route("api/print")]
public class RatePrinterController : ControllerBase
{
  private readonly IRateFetcherProvider _rateDataProvider;
  public RatePrinterController(IRateFetcherProvider rateDataProvider)
  {
    _rateDataProvider = rateDataProvider;
  }

  [HttpGet]
  public ActionResult<IEnumerable<ExchangeRateData>> GetAllRates()
  {
    return Ok(_rateDataProvider.GetRates().Values);
  }

  [HttpGet("{fromCurrency}/{toCurrency}")]
  public ActionResult<ExchangeRateData> GetSpecificRatePair(string fromCurrency, string toCurrency)
  {
    try
    {
      var specificRatePair = _rateDataProvider.GetSpecificRate(fromCurrency, toCurrency);
      return Ok(specificRatePair);
    }
    catch(Exception ex)
    {
      return NotFound(ex.Message);
    }
  }
}