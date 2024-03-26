using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SOFTITO_Project.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        [StringLength(200, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; } = "";

        [Column(TypeName = "smalldatetime")]
        public DateTime RegisterDate { get; set; }
        [StringLength(11, MinimumLength = 10)]
        [Column(TypeName = "varchar(11)")]
        public string TaxNumber { get; set; } = "";
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? WebAddress { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        public byte StateId { get; set; }
        [ForeignKey("StateId")]
        public State? State { get; set; }

    }
}
