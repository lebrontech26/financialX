using AutoMapper;
using Backend.Domain.Entities.Client;
using Backend.WebAPI.Hades.Features.Clients.DTOs.Get;
using Backend.WebAPI.Hades.Features.Clients.Scoring.DTOs;

namespace Backend.WebAPI.Hades.Features.Clients.Profiles
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            //Crear cliente
            CreateMap<ScoringFeaturesDTO, FeaturesBcra>();

            CreateMap<ScoreAjustesDTO, AjusteScoring>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.HistorialScoringId, opt => opt.Ignore());

            CreateMap<string, AlertaScoring>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.HistorialScoringId, opt => opt.Ignore())
                .ForMember(dest => dest.Texto, opt => opt.MapFrom(src => src));

            //Detalles cliente
            CreateMap<Cliente, ClienteDetalleResponse>()
                .ForMember(dest => dest.Calle, opt => opt.MapFrom(src => src.Domicilio.Calle))
                .ForMember(dest => dest.Localidad, opt => opt.MapFrom(src => src.Domicilio.Localidad))
                .ForMember(dest => dest.Provincia, opt => opt.MapFrom(src => src.Domicilio.Provincia))
                .ForMember(dest => dest.PerfilActual, opt => opt.MapFrom(src =>
                    src.HistorialesScoring.OrderByDescending(h => h.CalculadoEn).FirstOrDefault()))
                .ForMember(dest => dest.HistorialScoring, opt => opt.MapFrom(src =>
                    src.HistorialesScoring.OrderByDescending(h => h.CalculadoEn).ToList()));

            CreateMap<HistorialScoring, PerfilRiesgoDto>();

            CreateMap<HistorialScoring, HistorialScoringDto>()
                .ForMember(dest => dest.Alertas, opt => opt.MapFrom(src =>
                    src.Alertas.Select(a => a.Texto).ToList()));

            CreateMap<AjusteScoring, AjusteScoringDto>()
                .ForMember(dest => dest.TipoAjuste, opt => opt.MapFrom(src => src.Codigo));

            //Paginado
            CreateMap<Cliente, ClienteDTO>()
                .ForMember(dest => dest.PuntajeFinal, opt => opt.MapFrom(src =>
                    src.HistorialesScoring.OrderByDescending(h => h.CalculadoEn).FirstOrDefault() != null
                        ? src.HistorialesScoring.OrderByDescending(h => h.CalculadoEn).First().PuntajeFinal
                        : (int?)null))

                .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src =>
                    src.HistorialesScoring.OrderByDescending(h => h.CalculadoEn).FirstOrDefault() != null
                        ? src.HistorialesScoring.OrderByDescending(h => h.CalculadoEn).First().Categoria
                        : (CategoriaRiesgo?)null))

                .ForMember(dest => dest.SinEvidenciaCrediticia, opt => opt.MapFrom(src =>
                    src.HistorialesScoring.OrderByDescending(h => h.CalculadoEn).FirstOrDefault() != null
                        && src.HistorialesScoring.OrderByDescending(h => h.CalculadoEn).First().SinEvidenciaCrediticia));
        }
    }
}