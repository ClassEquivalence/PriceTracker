using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DataAccess.EFCore;
using PriceTracker.Models.Services.ShopService;
using PriceTracker.Models.Services.Mapping.MicroMappers;
using PriceTracker.Models.Services.MerchService;
using PriceTracker.Routing;
using PriceTracker.Models.DataAccess.Mapping;
using PriceTracker.Models.DataAccess.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using PriceTracker.Models.Services.ScrapingServices.ShopSpecificModels.Citilink;
using HtmlAgilityPack;
using PriceTracker.Models.DataAccess.Repositories.MerchRepository;

namespace PriceTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            //builder.Services.AddLogging();

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddControllersWithViews();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IShopService, ShopService>();
            builder.Services.AddSingleton<IMerchService, MerchService>();
            builder.Services.AddSingleton<IMerchToDtoMapper, MerchToDtoMapper>();
            builder.Services.AddSingleton<IShopToDtoMapper, ShopToDtoMapper>();
            builder.Services.AddSingleton<MerchRepository>();
            builder.Services.AddSingleton<ShopRepository>();
            builder.Services.AddSingleton<TimestampedPriceRepository>();
            builder.Services.AddSingleton<APILinkBuilder>();

            builder.Services.AddSingleton<PriceTrackerContext>();
            builder.Services.AddSingleton<DbContext>(sp => sp.GetRequiredService<PriceTrackerContext>());

            string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

            // TODO: Сделать потом нормальный конфиг подключения.
            //builder.Services.AddDbContext<PriceTrackerContext>(options => options.UseNpgsql(connection));

            builder.Services.AddSingleton<BidirectionalEntityModelMappingContext>();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI(); //opts => opts.SwaggerEndpoint("swagger", "v1"));

            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();

            app.Run();

        }
    }
}
