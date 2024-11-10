using AutoMapper;
using VFX.Application.Common.Models;
using VFX.Domain.Entities;

namespace VFX.Application.Common.Mappings;

// AutoMapper profile class that defines mappings between domain entities and DTOs
public class MapProfile : Profile
{
    public MapProfile()
    {
        // Mapping configurations for ForeignExchangeRate and ForeignExchangeRateDTO
        CreateMap<ForeignExchangeRate, ForeignExchangeRateDTO>()
            .ForMember(dest => dest.FromCurrencyCode, opt => opt.MapFrom(src => src.FromCurrency.Code))
            .ForMember(dest => dest.FromCurrencyName, opt => opt.MapFrom(src => src.FromCurrency.Name))
            .ForMember(dest => dest.ToCurrencyCode, opt => opt.MapFrom(src => src.ToCurrency.Code))
            .ForMember(dest => dest.ToCurrencyName, opt => opt.MapFrom(src => src.ToCurrency.Name))
            .ReverseMap();
    }
}
