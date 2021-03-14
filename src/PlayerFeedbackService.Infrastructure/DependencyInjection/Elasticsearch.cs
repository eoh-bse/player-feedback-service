using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;

using PlayerFeedbackService.DataAccess;

namespace PlayerFeedbackService.Infrastructure.DependencyInjection
{
    public static class ElasticsearchExtensions
    {
        private const int RetryAttempt = 10;

        public static void AddElasticsearch(this IServiceCollection services, Func<ElasticsearchConfig> createConfig)
        {
            var config = createConfig();

            var settings =
                new ConnectionSettings(config.Uri)
                    .DefaultIndex(config.DefaultIndex)
                    .MaximumRetries(RetryAttempt)
                    .MaxRetryTimeout(TimeSpan.FromSeconds(30.0));

            services.AddSingleton<IElasticClient>(new ElasticClient(settings));
        }

        public static void UseElasticsearch(this IApplicationBuilder app, Func<ElasticsearchConfig> createConfig)
        {
            var config = createConfig();
            var elasticClient = app.ApplicationServices.GetRequiredService<IElasticClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILogger<ElasticClient>>();

            logger.LogInformation($"Checking if default index '{config.DefaultIndex} exists'...");

            var defaultIndexExists = elasticClient.Indices.Exists(config.DefaultIndex).Exists;

            if (defaultIndexExists)
            {
                logger.LogInformation($"Default index '{config.DefaultIndex}' already exists. Not creating one.");
                logger.LogInformation($"Updating mapping for Default Index '{config.DefaultIndex}'...");

                elasticClient.Indices.PutMapping<PlayerFeedbackDocument>(mapper => mapper.AutoMap());

                logger.LogInformation($"Successfully updated mappings for {config.DefaultIndex}");

                return;
            }

            logger.LogInformation($"Default index '{config.DefaultIndex}' does not exist. Creating a new one...");

            elasticClient.Indices.Create(config.DefaultIndex, index =>
                index.Map<PlayerFeedbackDocument>(mapper => mapper.AutoMap<PlayerFeedbackDocument>())
            );

            logger.LogInformation($"Default index '{config.DefaultIndex}' is successfully created");
        }
    }
}
