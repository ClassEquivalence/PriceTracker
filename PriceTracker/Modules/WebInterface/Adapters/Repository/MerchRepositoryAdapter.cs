using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Entities.Domain.MerchPriceHistory;
using PriceTracker.Modules.WebInterface.API.DTOModels.Merch;
using System.Linq.Expressions;

namespace PriceTracker.Modules.WebInterface.Adapters.Repository
{
    public class MerchRepositoryAdapter: IMerchRepositoryAdapter
    {
        // TODO: много где хорошо бы сделать асинхронность.
        private readonly IDbContextFactory<PriceTrackerContext> _dbContextFactory;
        public MerchRepositoryAdapter(IDbContextFactory<PriceTrackerContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task<DetailedMerchDto?> GetMerchAsync(int merchId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var dto = context.Merches.Select(AsDetailedMerchDto).Single();
            return dto;
        }
        public async Task<DetailedMerchDto?> GetCitilinkMerchAsync(string citilinkMerchCode)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var merchDto = context.CitilinkMerches.Where(m => m.CitilinkId == citilinkMerchCode).
                Select(AsDetailedMerchDto).Single();
            return merchDto;
        }

        // 1. Выражение для TimestampedPrice
        private static readonly Expression<Func<TimestampedPriceEntity, TimestampedPriceDto>> AsTimestampedPriceDto =
            entity => new TimestampedPriceDto(
                entity.Id,
                entity.Price,
                entity.DateTime,
                entity.MerchPriceHistoryId
            );

        // 3. Главное выражение для DetailedMerchDto
        private static readonly Expression<Func<MerchEntity, DetailedMerchDto>> AsDetailedMerchDto =
            entity => new DetailedMerchDto(
                // Для вложенного объекта генерируем конструктор вручную, повторяя логику AsPriceHistoryDto
                new MerchPriceHistoryDto(
                    entity.PriceHistory.Id,
                    entity.PriceHistory.TimestampedPrices.AsQueryable().Select(AsTimestampedPriceDto).ToList(),
                    new TimestampedPriceDto(
                        entity.PriceHistory.CurrentPricePointer.CurrentPrice.Id,
                        entity.PriceHistory.CurrentPricePointer.CurrentPrice.Price,
                        entity.PriceHistory.CurrentPricePointer.CurrentPrice.DateTime,
                        entity.PriceHistory.CurrentPricePointer.CurrentPrice.MerchPriceHistoryId
                    ),
                    entity.PriceHistory.MerchId,
                    entity.PriceHistory.CurrentPricePointer.Id
                ),
                entity.Name,
                entity.ShopId,
                entity.Id
            );
    }
}
