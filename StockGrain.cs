using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Orleans;
using StockTickerOrleans.Interfaces;

namespace StockTickerOrleans
{
  public class StockGrain : Grain, IStockGrain
  {
    ///Docs <see cref="https://www.alphavantage.co/documentation/"/>
    // Request api key from here https://www.alphavantage.co/support/#api-key
    private static readonly string? ApiKey = ConfigurationManager.AppSettings.Get("api-key");
    private readonly HttpClient _httpClient = new();

    private string _price;

    public override async Task OnActivateAsync()
    {
      this.GetPrimaryKey(out var stock);
      await UpdatePrice(stock);

      RegisterTimer(
          UpdatePrice,
          stock,
          TimeSpan.FromMinutes(2),
          TimeSpan.FromMinutes(2));

      await base.OnActivateAsync();
    }

    private async Task UpdatePrice(object stock)
    {
      var priceTask = GetPriceQuote((string)stock);

      // read the results
      _price = await priceTask;
    }

    /// <summary>
    /// Returns price Info about stock ticker.
    /// </summary>
    /// <param name="stockTicker"></param>
    private async Task<string> GetPriceQuote(string stockTicker)
    {
      using var resp = await _httpClient.GetAsync(
        $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={stockTicker}&apikey={ApiKey}&datatype=csv");
      return await resp.Content.ReadAsStringAsync();
    }

    public Task<string> GetPrice() => Task.FromResult(_price);

    /// <summary>
    ///  Returns the annual inflation rates (consumer prices) of the United States.
    /// </summary>
    private async Task<string> GetUSAInflation()
		{
      using var response = await _httpClient.GetAsync($"https://www.alphavantage.co/query?function=INFLATION&apikey={ApiKey}");
      return await response.Content.ReadAsStringAsync();
		}
    /// <summary>
    /// This API returns the monthly inflation expectation data of the United States,
    /// as measured by the median expected price change next 12 months according to the Surveys of Consumers by University of Michigan.
    /// </summary>
    /// <returns></returns>
    private async Task<string> MonthlyInflationExpectation()
		{
      using var response = await _httpClient.GetAsync($"https://www.alphavantage.co/query?function=INFLATION_EXPECTATION&apikey={ApiKey}");
      return await response.Content.ReadAsStringAsync();
		}
    /// <summary>
    /// Returns the monthly unemployment data of the United States.
    /// The unemployment rate represents the number of unemployed as a percentage of the labor force.
    /// Labor force data are restricted to people 16 years of age and older, who currently reside in 
    /// 1 of the 50 states or the District of Columbia, who do not reside in institutions 
    /// (e.g., penal and mental facilities, homes for the aged), and who are not on active duty in the Armed Forces
    /// </summary>
    private async Task<string> UnemploymetRate()
		{
      using var response = await _httpClient.GetAsync($"https://www.alphavantage.co/query?function=UNEMPLOYMENT&apikey={ApiKey}");
      return await response.Content.ReadAsStringAsync();
		}
    /// <summary>
    /// Returns the daily, weekly, and monthly federal funds rate (interest rate) of the United States
    /// </summary>
    /// <param name="timeframe">By default, interval=monthly. Strings daily, weekly, and monthly are accepted</param>
    /// <returns></returns>
    private async Task<string> MonthlyInterestRate(string timeframe = "monthly")
		{
      using var response = await _httpClient.GetAsync($"https://www.alphavantage.co/query?function=FEDERAL_FUNDS_RATE&interval={timeframe}&apikey={ApiKey}");
      return await response.Content.ReadAsStringAsync();
		}
    /// <summary>
    /// This API returns the annual and quarterly Real GDP of the United States.
    /// </summary>
    /// <param name="interval">By default, interval=annual. Strings quarterly and annual are accepted.</param>
    /// <returns></returns>
    private async Task<string> USAGdp(string interval = "annual")
    {
      using var response = await _httpClient.GetAsync($"https://www.alphavantage.co/query?function=REAL_GDP&interval={interval}&apikey={ApiKey}");
      return await response.Content.ReadAsStringAsync();
    }
    /// <summary>
    /// Returns monthly time series (last trading day of each month, monthly open, monthly high,
    /// monthly low, monthly close, monthly volume) of the global equity specified, covering 20+ years of historical data.
    /// </summary>
    /// <param name="symbol">The name of the equity of your choice. For example: symbol=IBM</param>
    private async Task<string> TimeSeriesMonthly(string symbol)
    {
      using var response = await _httpClient.GetAsync($"https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol={symbol}&apikey={ApiKey}");
       var resp =await response.Content.ReadAsStringAsync();
      return resp;
    }



  }
}