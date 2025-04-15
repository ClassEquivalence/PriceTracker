using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DTOModels;
using System.Text.RegularExpressions;

namespace PriceTracker.Models.Services.Mapping
{
    public interface IDomainToDtoMapper <TDomain, TDto> where TDomain: BaseModel where TDto : BaseDTO
    {
        public TDto ModelToDTO(TDomain domainModel);
        public TDomain DTOToModel(TDto DTO);
    }
}
