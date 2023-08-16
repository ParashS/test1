using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Net;
using System.Text;
using System.Text.Json;
using Vessels.ApiModels;

namespace Vessels.Extensions
{
  public class ErrorDetails : CommonApiResponse
  {
    public override string ToString()
    {
      return JsonSerializer.Serialize(this);
    }
  }

  public static class ExceptionMiddlewareExtensions
  {
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
      _ = app.UseExceptionHandler(appError =>
      {
        appError.Run(async context =>
        {
          context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
          context.Response.ContentType = "application/json";
          var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
          if (contextFeature != null)
          {
            // Serilog logging
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Error - " + DateTime.UtcNow.ToLongDateString());
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append("Path : " + contextFeature.Path);
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append("Error Exception : " + contextFeature.Error.Message);
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append("Inner Exception : " + contextFeature.Error.InnerException?.Message ?? "");
            stringBuilder.Append(Environment.NewLine);

            Log.Information(stringBuilder.ToString());
            Log.Error(contextFeature.Error, "Error Stacktrace");

            Log.Information(Environment.NewLine);
            Log.Information(Environment.NewLine);

            await context.Response.WriteAsync(new ErrorDetails()
            {
              data = null,
              statusCode = context.Response.StatusCode,
              message = "Internal Server Error."
            }.ToString());
          }
        });
      });
    }
  }
}
