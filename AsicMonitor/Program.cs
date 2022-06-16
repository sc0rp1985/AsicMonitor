// See https://aka.ms/new-console-template for more information
using AsicMonitor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Monitor.Classes;
using Monitor.Classes.Impl;
using Monitor.Common;
using Monitor.Const;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Unity;

Console.WriteLine("Hello, World!");


var host = CreateDefaultBuilder().Build();
// Invoke Worker
using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;
var workerInstance = provider.GetRequiredService<Worker>();
workerInstance.DoWork();
host.Run();


static IHostBuilder CreateDefaultBuilder()
{
    return Host.CreateDefaultBuilder()        
        .ConfigureServices(services =>
        {
            services.AddSingleton<Worker>();
            services.AddSingleton<IUnityContainer>(cfg => InitializeContainer());
        });
}

static IUnityContainer InitializeContainer()
{
    var cfg = new UnityContainer();
    cfg
        .RegisterInstance<IUnityContainer>(cfg)
        .Monitor_Classes();
    IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();
    var connectionConfig = config.GetRequiredSection("ConnectionConfig").Get<СonnectionConfig>();                                                                             
    cfg.RegisterInstance(connectionConfig);
    return cfg;
}




