using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DdtcApi.Models
{
    public class Translation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Banner { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Image { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("LinkPC")]
        public string LinkPc { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string LinkMobile { get; set; } = string.Empty;
    }
}
