using AutoMapper;
using Magic_villa_web.DTO;
using Magic_villa_web.Models.DTO;


namespace Magic_villa_web
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();
            CreateMap<VillaDTO, VillaCreateDTO>().ReverseMap();
            CreateMap<VillaNumberDTO, VillaNumberCreateDTO>().ReverseMap();
            CreateMap<VillaNumberDTO, VillaNumberUpdateDTO>().ReverseMap();
        }
    }
}
