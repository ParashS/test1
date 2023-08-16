using DataTransferObjects.MailSettings;
using SendGrid.Extensions.DependencyInjection;
using Serilog;
using Vessels.Extensions;
using Vessels.ScheduleJob;
using Vessels.ScheduleJob.VesLinkAPI;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().WriteTo.File("..\\Logs\\Vessels-.log", rollingInterval: RollingInterval.Day).CreateLogger();

builder.Logging.ClearProviders();

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();

builder.Services.ConfigureSQLContext(builder.Configuration);
builder.Services.ConfigureRepositoryWrapper();
builder.Services.ConfigureServiceManager();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//CronJobs
builder.Services.AddCronJob<VesLinkAPICall>(c =>
{
    c.TimeZoneInfo = TimeZoneInfo.Local;
    c.CronExpression = builder.Configuration.GetSection("ScheduleJobConfig:CallVesLinkAPITiming").Value;
});
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

//SendGrid Mail service
builder.Services.AddSendGrid(options =>
{
    options.ApiKey = builder.Configuration
    .GetSection("MailSettings").GetValue<string>("APIKey");
});
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions()
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();