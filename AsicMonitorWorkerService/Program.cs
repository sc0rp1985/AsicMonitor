using AsicMonitorWorkerService;
using Monitor.Classes.Impl;
using Unity;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "AsicMonitor.Alarm.Service";
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<IUnityContainer>(InitializeContainer());
        services.AddHostedService<Worker>();
    })    
    .Build();

await host.RunAsync();



static IUnityContainer InitializeContainer()
{
    var cfg = new UnityContainer();
    cfg
        .RegisterInstance<IUnityContainer>(cfg)
        .Monitor_Classes();
    return cfg;
}