using System.Threading.Tasks;
using Orleans;

namespace StockTickerOrleans.Interfaces
{
  public interface IStockGrain : IGrainWithStringKey
  {
    Task<string> GetPrice();
  }
}
