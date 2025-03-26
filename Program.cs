using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using PricesBotWorkerService;



HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = ".NET Telegram bot service";
});

LoggerProviderOptions.RegisterProviderOptions<
    EventLogSettings, EventLogLoggerProvider>(builder.Services);
builder.Services.AddSingleton<BotService>();

builder.Services.AddHostedService<WindowsBackgroundService>();
builder.Services.AddOptions<AppSettings>().BindConfiguration("AppSettings");
builder.Services.Configure<EventLogSettings>(config =>
{
    config.SourceName = "EnergoPriceBotService";
    config.LogName = "EnergoPriceBotService";

});
IHost host = builder.Build();
host.Run();
