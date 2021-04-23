using MetricsManagerClient.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Quartz;
using Quartz.Impl;
using MetricsManagerClient.Infrastructure;
using MetricsManagerClient.Jobs;
using MetricsManagerClient.Responces;
using MetricsManagerClient.Model;

namespace MetricsManagerClient
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public App()
        {
            _host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();
        }

        private void ConfigureServices(IServiceCollection service)
        {
            service.AddSingleton<MainWindow>();

            service.AddSingleton<AllCpuMetricsApiResponse>();

            service.AddSingleton<IJobFactory, SIngletoneJobFactory>();
            service.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            service.AddSingleton<CpuMetricJob>();
            service.AddSingleton(new JobSchedule(
                jobType: typeof(CpuMetricJob),
                cronExpression: "0/5 * * * * ?"));

            service.AddHostedService<QuartzHostedService>();

            service.AddHttpClient<IMetricsClient, MetricsClient>();
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            using (var serviceScope = _host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                try
                {
                    await _host.StartAsync();
                    var mainWindow = services.GetRequiredService<MainWindow>();
                    mainWindow.Show();
                }
                catch 
                {

                }
            }
        }
    }
}
