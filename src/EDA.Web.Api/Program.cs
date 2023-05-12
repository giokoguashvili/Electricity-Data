
using EDA.Web.Api.Domain;
using static System.Net.WebRequestMethods;

namespace EDA.Web.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {


            new DownloadElectricityDatasets(
                new string[]
                {
                    "https://data.gov.lt/dataset/1975/download/10766/2022-05.csv",
                    //"https://data.gov.lt/dataset/1975/download/10765/2022-04.csv",
                    //"https://data.gov.lt/dataset/1975/download/10764/2022-03.csv",
                    "https://data.gov.lt/dataset/1975/download/10763/2022-02.csv"
                },
                @"C:\Electricity-Data\"
            ).Do();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

            app.Run();
        }
    }
}