using MagicVilla_API.DTO;

namespace MagicVilla_API.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> VillaList = new List<VillaDTO>()
        {
                new VillaDTO { Id =1, Name="alex"},
                new VillaDTO {Id = 2, Name ="suez"}
        };
    }
}
