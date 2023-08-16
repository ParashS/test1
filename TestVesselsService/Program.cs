using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using ServiceContracts;
using Services;
using TestVesselsService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        //services.AddDbContext<ShipWatchDataContext>(options => options.UseSqlServer("Server=DESKTOP-GUAILVD;Database=ShipWatchData;User ID=sa;Password=123456;MultipleActiveResultSets=true;", SqlTypeOption => SqlTypeOption.UseNetTopologySuite()));
        //services.AddSingleton<IRepositoryWrapper, RepositoryWrapper>();
        //services.AddSingleton<IServiceManager, ServiceManager>();
    })
    .Build();

await host.RunAsync();
