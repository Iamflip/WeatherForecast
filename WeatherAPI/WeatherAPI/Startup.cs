using AutoMapper;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using MetricsInfrastucture;
using MetricsInfrastucture.Interfaces;
using MetricsManager.Client;
using Polly;
using System;
using Microsoft.Data.Sqlite;
using MetricsManager.Jobs;
using MetricsManager.Metrics;
using MetricsManager.DAL;
using System.Data.SQLite;
using System.Reflection;
using System.IO;

namespace MetricsManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private const string ConnectionString = @"Data Source=metrics.db; Version=3;";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            ConfigureSqlLiteConnection(services);

            services.AddSingleton<IRepositoryMM<CpuMetric>, CpuMetricsRepository>();
            services.AddSingleton<IRepositoryMM<DotNetMetric>, DotNetMetricsRepository>();
            services.AddSingleton<IRepositoryMM<HddMetric>, HddMetricsRepository>();
            services.AddSingleton<IRepositoryMM<NetworkMetric>, NetworkMetricsRepository>();
            services.AddSingleton<IRepositoryMM<RamMetric>, RamMetricsRepository>();
            services.AddSingleton<IAgentsRepository<AgentInfo>, AgentsRepository>();

            var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile()));
            var mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);

            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                 // ��������� ��������� SQLite 
                    .AddSQLite()
                 // ������������� ������ �����������
                        .WithGlobalConnectionString(ConnectionString)
                  // ������������ ��� ������ ������ � ����������
                        .ScanIn(typeof(Startup).Assembly).For.Migrations()
                             ).AddLogging(lb => lb
                             .AddFluentMigratorConsole());

            services.AddSingleton<CpuMetricJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(CpuMetricJob),
                cronExpression: "0/10 * * * * ?"));

            services.AddSingleton<RamMetricJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(RamMetricJob),
                cronExpression: "0/10 * * * * ?"));

            services.AddSingleton<DotNetMetricJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(DotNetMetricJob),
                cronExpression: "0/10 * * * * ?"));

            services.AddSingleton<HddMetricJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(HddMetricJob),
                cronExpression: "0/10 * * * * ?"));

            services.AddSingleton<NetworkMetricJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(NetworkMetricJob),
                cronExpression: "0/10 * * * * ?"));

            services.AddHostedService<QuartzHostedService>();

            services.AddHttpClient<IMetricsAgentClient, MetricsAgentClient>()
              .AddTransientHttpErrorPolicy(p =>
                  p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(1000)));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API ������� ������ ����� ������",
                    Contact = new OpenApiContact
                    {
                        Name = "Rodion",
                        Email = "kim.Radik@mail.ru"
                    }
                });
                // ��������� ���� �� �������� ����� ����������� ��� Swagger UI
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

        }

        private void ConfigureSqlLiteConnection(IServiceCollection services)
        {
            string connectionString = "Data Source=:memory:";
            var connection = new SqliteConnection(connectionString);
            connection.Open();
            services.AddSingleton(connection);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationRunner migrationRunner)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherAPI v1");
                    c.RoutePrefix = string.Empty;
                });
                
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            migrationRunner.MigrateUp();
        }
    }
}
