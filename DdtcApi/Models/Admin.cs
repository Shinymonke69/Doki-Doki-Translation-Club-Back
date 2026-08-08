using System.ComponentModel.DataAnnotations;

namespace DdtcApi.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Key { get; set; } = string.Empty;
    }
}
