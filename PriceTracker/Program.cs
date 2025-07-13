using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataProvider;
using PriceTracker.Modules.Repository.DataAccess.EFCore;

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



            DependencyInjector.InjectRepositoryDependencies(builder.Services);
            DependencyInjector.InjectWebInterfaceDependencies(builder.Services);
            DependencyInjector.InjectMerchDataProviderDependencies(builder.Services);

            string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

            // TODO: Сделать потом нормальный конфиг подключения.
            //builder.Services.AddDbContext<PriceTrackerContext>(options => options.UseNpgsql(connection));


            // Добавим политику CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev", policy =>
                {
                    policy.WithOrigins("https://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            //app.Services.GetService<IMerchDataProviderFacade>()?.ProcessMerchUpsertion();

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
                // Применяем CORS перед MapControllers
                app.UseCors("AllowAngularDev");
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
