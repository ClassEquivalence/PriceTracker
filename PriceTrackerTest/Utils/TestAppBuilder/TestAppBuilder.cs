using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PriceTracker;
using PriceTracker.Core.Configuration.ProvidedWithDI;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Core.Utils.DependencyInjection;
using PriceTrackerTest.Utils.TestAppBuilder.Repository;

namespace PriceTrackerTest.Utils.TestAppBuilder
{

    public enum RepositoryType { InMemory_Test }

    [Flags]
    public enum ConstructableComponents
    {
        None = 0,
        Repository = 1 << 0,
        Logger = 1 << 1,
        Options = 1 << 2,
        MerchDataMaintenance = 1 << 3,
        All = (1 << 4) - 1
    }

    public class TestAppBuilder
    {
        private ConstructableComponents constructedComponents;


        private readonly ServiceCollection _services;
        public TestAppBuilder()
        {
            _services = new ServiceCollection();
            constructedComponents = ConstructableComponents.None;
        }


        public TestAppBuilder WithRepository(RepositoryType type)
        {
            if (constructedComponents.HasFlag(ConstructableComponents.Repository))
                throw new InvalidOperationException();

            switch (type)
            {
                case RepositoryType.InMemory_Test:
                    RepositoryConstructor<PriceTrackerTestContext> constructor = new();
                    constructor.AddRepository(_services);
                    break;
            }
            constructedComponents |= ConstructableComponents.Repository;
            return this;
        }

        public TestAppBuilder WithLogger(ILogger logger)
        {
            if (constructedComponents.HasFlag(ConstructableComponents.Logger))
                throw new InvalidOperationException();
            _services.AddLogging();
            _services.AddSingleton<ILogger>(s => s.GetService<ILogger<Program>>());
            _services.AddSingleton<ILogger>(logger);
            constructedComponents |= ConstructableComponents.Logger;
            return this;
        }
        public TestAppBuilder WithOptions()
        {
            if (constructedComponents.HasFlag(ConstructableComponents.Options))
                throw new InvalidOperationException();
            ConfigurationManager configurationManager = new();
            configurationManager.AddJsonFile("appsettings.json"); // Check if it adds this file

            _services.Configure<MerchUpsertionOptions>(
                configurationManager.GetSection(MerchUpsertionOptions.OptionsSectionKey));
            _services.Configure<CitilinkUpsertionOptions>(
                configurationManager.GetSection($"{MerchUpsertionOptions.OptionsSectionKey}:CitilinkUpsertionOptions"));
            _services.AddSingleton<IAppEnvironment, HostEnvironmentAdapter>();

            constructedComponents |= ConstructableComponents.Options;
            return this;
        }

        public TestAppBuilder WithMerchDataMaintenance()
        {
            if (constructedComponents.HasFlag(ConstructableComponents.MerchDataMaintenance))
                throw new InvalidOperationException();
            DependencyInjector.InjectMerchDataMaintenanceDependencies(_services);
            constructedComponents |= ConstructableComponents.MerchDataMaintenance;
            return this;
        }

        public TestApp Build()
        {
            if (constructedComponents != ConstructableComponents.All)
            {
                throw new InvalidOperationException();
            }
            else
            {
                var serviceProvider = _services.BuildServiceProvider();
                return new TestApp(serviceProvider);
            }
        }

        public TestApp BuildWithDefaults(ILogger logger)
        {
            if (constructedComponents != ConstructableComponents.None)
                throw new InvalidOperationException();
            return this.WithRepository(RepositoryType.InMemory_Test)
                .WithLogger(logger).WithOptions().WithMerchDataMaintenance()
                .Build();
        }


        /*
        
        public CitilinkDataAugmenter AssembleAugmenter(UsedDatabase database,
            ILogger logger)
        {
            ServiceCollection collection = new();

            CitilinkUpsertionOptions upsertionOptions = new()
            {
                CitilinkUrl = "https://www.citilink.ru/"
            };

            IOptions<CitilinkUpsertionOptions> interfacedOptions =
                Options.Create<CitilinkUpsertionOptions>(upsertionOptions);

            collection.AddSingleton(interfacedOptions);

            collection.AddSingleton<ILogger>(logger);

            Mock<ICitilinkMerchRepositoryFacade> mockMerchRepositoryFacade = new();
            mockMerchRepositoryFacade.Setup(r => r.UpdateManyAsync
            (It.IsAny<List<CitilinkMerchDto>>())).Callback<List<CitilinkMerchDto>>
            (async list =>
            {
                _outputHelper.WriteLine("Merch UpdateMany List starts ---");
                foreach (var dto in list)
                {
                    _outputHelper.WriteLine($"{dto.Name}, {dto.CitilinkId}, " +
                        $"{dto.PriceTrack.CurrentPrice}");
                }
                _outputHelper.WriteLine("Merch UpdateMany List ends ---");
            });

            mockMerchRepositoryFacade.Setup(r => r.CreateManyAsync
            (It.IsAny<List<CitilinkMerchDto>>())).Callback<List<CitilinkMerchDto>>
            (async list =>
            {
                _outputHelper.WriteLine("Merch CreateMany List starts ---");
                foreach (var dto in list)
                {
                    _outputHelper.WriteLine($"{dto.Name}, {dto.CitilinkId}, " +
                        $"{dto.PriceTrack.CurrentPrice}");
                }
                _outputHelper.WriteLine("Merch CreateMany List ends ---");
            });

            mockMerchRepositoryFacade.Setup(r => r.GetMultiple(It.IsAny<string[]>()))
                .Returns([]);

            var shopRepositoryMock = new Mock<IShopRepositoryFacade>();


            // Это успешный сетап мока? сомневаюсь хз посмотрим.
            shopRepositoryMock.Setup(r => r.SingleOrDefault(It.IsAny<Func<ShopDto, bool>>()))
                .Returns(new ShopDto(1, "Citilink", []));


            var catalogTreeRepositoryMock = new Mock<ICitilinkCatalogTreeRepositoryAdapter>();

            catalogTreeRepositoryMock.Setup(r => r.UpdateNodeAsync(It.IsAny<CitilinkNode>()));
            catalogTreeRepositoryMock.Setup(r => r.AppendBranchesAsync(It.IsAny<CitilinkNode>(),
                It.IsAny<List<CitilinkScrapeTarget>>()));

            var merchRepositoryPseudo = mockMerchRepositoryFacade.Object;

            var catalogTreeRepoPseudo = catalogTreeRepositoryMock.Object;

            var shopRepositoryPseudo = shopRepositoryMock.Object;

            collection.AddSingleton<ICitilinkMerchRepositoryFacade>(merchRepositoryPseudo);
            collection.AddSingleton<ICitilinkCatalogTreeRepositoryAdapter>(catalogTreeRepoPseudo);
            collection.AddSingleton<IShopRepositoryFacade>(shopRepositoryPseudo);

            DependencyInjector.InjectMerchDataMaintenanceDependencies(collection);
            var provider = collection.BuildServiceProvider();
        }

         */


    }
}
