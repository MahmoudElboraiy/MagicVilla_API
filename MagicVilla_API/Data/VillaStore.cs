using MagicVilla_API.DTO;

namespace MagicVilla_API.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> VillaList = new List<VillaDTO>()
        {
                new VillaDTO { Id =1, Name="alex",Sqft=30,Occupancy=300},
                new VillaDTO {Id = 2, Name ="suez",Sqft=10,Occupancy=402}
        };
    }
}
