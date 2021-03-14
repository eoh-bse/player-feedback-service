using System;
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
using PlayerFeedbackService.DataAccess;

namespace PlayerFeedbackService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IClock, Clock>();
            services.AddScoped<IPlayerFeedbackRepository, PlayerFeedbackRepository>();
            services.AddScoped<IQueryHandler, QueryHandler>();
            services.AddScoped<IFeedbackSender, FeedbackSender>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlayerFeedbackService", Version = "v1" });
            });

            services.AddElasticsearch(() => new ElasticsearchConfig
            {
                DefaultIndex = Configuration["Elasticsearch:DefaultIndex"],
                Uri = new Uri(Configuration["Elasticsearch:Uri"])
            });

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
