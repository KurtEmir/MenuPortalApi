using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SOFTITO_Project.Models
{
    public class State
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte Id { get; set; }
        [Required]
        [StringLength(10)]
        [Column(TypeName = "nvarchar(10)")]
        public string Name { get; set; } = "";
    }
}
