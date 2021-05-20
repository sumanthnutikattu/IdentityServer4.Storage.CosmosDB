﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Storage.CosmosDB.Interfaces;
using IdentityServer4.Storage.CosmosDB.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Storage.CosmosDB
{
    /// <summary>
    ///     Token Cleanup Class.
    /// </summary>
    public class TokenCleanup
    {
        private readonly TimeSpan _interval;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource _source;

        /// <summary>
        ///     Create an instance of the TokenCleanup Class.
        /// </summary>
        /// <param name="serviceProvider">Instance of the Service Provider.</param>
        /// <param name="logger">Instance of the Logger.</param>
        /// <param name="options">Instance of the Token Cleanup Options.</param>
        /// <exception cref="ArgumentNullException">Is thrown when serviceProvider, logger or options is null.</exception>
        /// <exception cref="ArgumentException">Is thrown when options.interval is less than 1.</exception>
        public TokenCleanup(IServiceProvider serviceProvider, 
            ILogger<TokenCleanup> logger, 
            TokenCleanupOptions options)
        {
            Guard.ForNull(serviceProvider, nameof(serviceProvider));
            Guard.ForNull(logger, nameof(logger));
            Guard.ForNull(options, nameof(options));
            Guard.ForValueLessThan(options.Interval, 1, nameof(options.Interval));

            _serviceProvider = serviceProvider;
            _logger = logger;
            _interval = TimeSpan.FromSeconds(options.Interval);
        }

        /// <summary>
        ///     Starts the process for cleaning up the tokens.
        /// </summary>
        /// <exception cref="InvalidOperationException">Is thrown when the process is already running.</exception>
        public void Start()
        {
            if (_source != null) throw new InvalidOperationException($"Already started, call `{nameof(Stop)}` first.");

            _logger.LogDebug("Starting token cleanup.");

            _source = new CancellationTokenSource();
            Task.Factory.StartNew(() => Start(_source.Token));
        }

        /// <summary>
        ///     Stops the process for cleaning up the tokens.
        /// </summary>
        /// <exception cref="InvalidOperationException">Is thrown when the process is not running.</exception>
        public void Stop()
        {
            if (_source == null) throw new InvalidOperationException($"Not started, call `{nameof(Start)}` first.");

            _logger.LogDebug("Stopping token cleanup.");
            _source.Cancel();
            _source = null;
        }

        private async Task Start(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested");
                    break;
                }

                try
                {
                    await Task.Delay(_interval, cancellationToken);
                }
                catch
                {
                    _logger.LogDebug("Task.Delay exception. exiting.");
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested");
                    break;
                }

                await ClearTokens();
            }
        }

        private async Task ClearTokens()
        {
            try
            {
                _logger.LogTrace("Querying for tokens to clear");

                using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (var context = serviceScope.ServiceProvider.GetService<IPersistedGrantDbContext>())
                    {
                        var expired = context.PersistedGrants().ToList().Where(x => x.Expiration < DateTime.UtcNow).ToList();

                        _logger.LogDebug("Clearing {tokenCount} tokens", expired.Count);

                        if (expired.Count > 0) await context.RemoveExpired();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception cleaning tokens {exception}", ex.Message);
            }
        }
    }
}