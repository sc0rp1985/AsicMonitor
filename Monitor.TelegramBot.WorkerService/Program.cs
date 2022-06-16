using Monitor.Classes;
using Monitor.Classes.Impl;
using Monitor.TelegramBot.WorkerService;
using Unity;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "AsicMonitor.Telegram.Bot";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IUnityContainer>(cfg => InitializeContainer());
    })
    .Build();

await host.RunAsync();

static IUnityContainer InitializeContainer()
{
    var cfg = new UnityContainer();
    cfg
        .RegisterInstance<IUnityContainer>(cfg)
        .Monitor_Classes();
    IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();
    var connectionConfig = config.GetRequiredSection("ConnectionConfig").Get<ÑonnectionConfig>();
    var tgConfig = config.GetRequiredSection("TelegramConfig").Get<TelegramConfig>();
    cfg.RegisterInstance(connectionConfig);
    cfg.RegisterInstance(tgConfig);
    return cfg;
}



