using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DTOModels;

namespace PriceTracker.Models.Services.Mapping.Extensions
{
    public static class DomainToDtoMapperExtension
    {
        //<TDomain, TDto> where TDomain : BaseModel where TDto : BaseDTO
        public static List<TDto> ModelsToDTOs<TDomain, TDto>(this IDomainToDtoMapper<TDomain, TDto> mapper, List<TDomain> domainModels)
                where TDomain : BaseDomain where TDto : BaseDTO
        {
            return domainModels.Select(mapper.ModelToDTO).ToList();
        }
        public static List<TDomain> DTOsToModels<TDomain, TDto>(this IDomainToDtoMapper<TDomain, TDto> mapper, List<TDto> DTOs)
            where TDomain : BaseDomain where TDto : BaseDTO
        {
            return DTOs.Select(mapper.DTOToModel).ToList();
        }
    }
}
