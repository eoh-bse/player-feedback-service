using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using PlayerFeedbackService.Infrastructure;
using PlayerFeedbackService.Infrastructure.DependencyInjection;
using PlayerFeedbackService.Service;
using PlayerFeedbackService.Service.Abstractions;
using PlayerFeedbackService.Service.DataAccess;
using PlayerFeedbackService.Service.MessageBroker;
using PlayerFeedbackService.MessageBroker;
using PlayerFeedbackService.MessageBroker.MessageHandlers;
using PlayerFeedbackService.MessageBroker.MessageHandlers.Abstractions;
using PlayerFeedbackService.MessageBroker.MessageListeners;
using PlayerFeedbackService.DataAccess;

namespace PlayerFeedbackService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private readonly MessageBrokerConfig _messageBrokerConfig;
        private readonly ImmutableDictionary<string, Topic> _topics;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _messageBrokerConfig = ExtractMessageBrokerConfig(configuration);
            _topics = ExtractTopics(configuration);
        }

        private MessageBrokerConfig ExtractMessageBrokerConfig(IConfiguration configuration)
        {
            return new MessageBrokerConfig
            {
                Servers = configuration["Kafka:BootstrapServers"]
            };
        }

        private ImmutableDictionary<string, Topic> ExtractTopics(IConfiguration configuration)
        {
            var topicsSection = configuration.GetSection("Kafka:Topics").GetChildren();

            return topicsSection.Select(topic =>
            {
                var topicName = topic["Name"];
                var messageName = topic["MessageName"];
                var deadLetterQueueName = topic["DeadLetterQueueName"];
                var groupIds = topic.GetSection("GroupIds").Get<Dictionary<string, string>>();

                return new Topic
                {
                    Name = topicName,
                    For = messageName,
                    DeadLetterQueueName = deadLetterQueueName,
                    GroupIds = groupIds.ToImmutableDictionary()
                };
            }).ToImmutableDictionary(topic => topic.For, topic => topic);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IClock, Clock>();
            services.AddScoped<IQueryHandler, QueryHandler>();
            services.AddScoped<IFeedbackSender, FeedbackSender>();
            services.AddSingleton<IPlayerFeedbackRepository, PlayerFeedbackRepository>();
            services.AddSingleton<IAddPlayerFeedbackMessageHandlers, AddPlayerFeedbackMessageHandlers>();
            services.AddSingleton<IMessageSender, MessageSender>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlayerFeedbackService", Version = "v1" });
                c.OperationFilter<AddRequiredUbiUserIdHeaderParameter>();
            });

            services.AddElasticsearch(() => new ElasticsearchConfig
            {
                DefaultIndex = Configuration["Elasticsearch:DefaultIndex"],
                Uri = new Uri(Configuration["Elasticsearch:Uri"])
            });

            services.AddMessageBrokerConfigProvider(_messageBrokerConfig, _topics);
            services.AddKafkaProducer(() => new MessageBrokerConfig
            {
                Servers = Configuration["Kafka:BootstrapServers"]
            });

            services.CreateTopicsOnMessageBroker(_messageBrokerConfig, _topics);

            services.AddHostedService<StorePlayerFeedback>();

            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlayerFeedbackService v1")
                );
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseElasticsearch(() => new ElasticsearchConfig
            {
                DefaultIndex = Configuration["Elasticsearch:DefaultIndex"],
                Uri = new Uri(Configuration["Elasticsearch:Uri"])
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
