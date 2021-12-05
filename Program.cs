using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using StockTickerOrleans.Services;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(builder =>
    {
      builder.UseLocalhostClustering();
    })
    .ConfigureServices(services =>
    {
      services.AddHostedService<StocksHostedService>();
    })
    .RunConsoleAsync();