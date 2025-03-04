using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_API.Models
{
    public class VillaNumber
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillaNo { get; set; }
        [ForeignKey("villa")]
        public int VillaId { get; set; }
        public Villa villa { get; set; }
        public string SpecialDetails { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
