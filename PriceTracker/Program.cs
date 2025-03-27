using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.BaseAppModels;
using PriceTracker.Models.BaseAppModels.ShopCollections;
using PriceTracker.Models.DbRelatedModels;

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
            builder.Services.AddSingleton<IShopCollection, ShopsFromDatabase>();
            builder.Services.AddSingleton<PriceTrackerContext>();

            //Не уверен что это правильно.
            builder.Services.AddSingleton<ICollection<Shop>, DbDataExtractor<Shop>>();

            //builder.Services.AddSingleton<ICollection<Shop>, DbDataExtractor<Shop>>();

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
