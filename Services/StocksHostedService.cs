﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using StockTickerOrleans.Interfaces;

namespace StockTickerOrleans.Services
{
  public class StocksHostedService : BackgroundService
  {
    private readonly ILogger<StocksHostedService> _logger;
    private readonly IClusterClient _client;
    private readonly List<string> _symbols = new List<string> { "MSFT", "GOOG", "AAPL", "GME", "AMZN" };

    public StocksHostedService(ILogger<StocksHostedService> logger, IClusterClient client)
    {
      _logger = logger;
      _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          _logger.LogInformation("Fetching stock prices");
          var tasks = new List<Task<string>>();

          // Fan out calls to each of the stock grains
          foreach (var symbol in _symbols)
          {
            var stockGrain = _client.GetGrain<IStockGrain>(symbol);
            tasks.Add(stockGrain.GetPrice());
          }

          // Collect the results
          await Task.WhenAll(tasks);

          // Print the results
          foreach (var task in tasks)
          {
            var price = await task;
            _logger.LogInformation("Price is {Price}", price);
          }

          await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
        catch (Exception error) when (!stoppingToken.IsCancellationRequested)
        {
          _logger.LogError(error, "Error fetching stock price");
        }
      }
    }
  }
}