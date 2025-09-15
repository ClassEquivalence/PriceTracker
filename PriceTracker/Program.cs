using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Configuration.ProvidedWithDI;
using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataProvider;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;

namespace PriceTracker
{
    public class Program
    {
        public static async Task Main(string[] args)
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

            // TODO: Сделать потом нормальный конфиг подключения.
            //string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
            //builder.Services.AddDbContext<PriceTrackerContext>(options => options.UseNpgsql(connection));


            builder.Services.Configure<ProductionDbOptions>(
                builder.Configuration.GetSection(ProductionDbOptions.OptionsSectionKey));

            builder.Services.Configure<MerchUpsertionOptions>(
                builder.Configuration.GetSection(MerchUpsertionOptions.OptionsSectionKey));


            if (builder.Environment.IsDevelopment())
            {
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
            }

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
                // Применяем CORS перед MapControllers
                app.UseCors("AllowAngularDev");
            }



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();


            app.MapFallbackToFile("index.html");

            app.Services.GetService<IRepositoryFacade>()?.EnsureRepositoryInitialized();

            //var upsertionTask = app.Services.GetService<IMerchDataProviderFacade>()?.
            //    ProcessMerchUpsertion();




            var appTask = app.RunAsync();

            //await upsertionTask;
            await appTask;

        }
    }
}
