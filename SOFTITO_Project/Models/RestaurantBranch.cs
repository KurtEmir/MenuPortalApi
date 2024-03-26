using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOFTITO_Project.Models
{
    public class RestaurantBranch
    {
        public int Id { get; set; }

        [StringLength(200, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; } = "";

        [StringLength(5, MinimumLength = 5)]
        [Column(TypeName = "char(5)")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; } = "";

        [StringLength(200, MinimumLength = 5)]
        [Column(TypeName = "nvarchar(200)")]
        public string Address { get; set; } = "";

        [StringLength(30)]
        [Column(TypeName = "varchar(30)")]
        public string Phone { get; set; } = "";

        [Column(TypeName = "smalldatetime")]
        public DateTime RegisterDate { get; set; }
        [StringLength(11, MinimumLength = 10)]
        [Column(TypeName = "varchar(11)")]
        public string TaxNumber { get; set; } = "";
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? WebAddress { get; set; }

        public int RestaurantId { get; set; }
        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }

        public byte StateId { get; set; }
        [ForeignKey("StateId")]
        public State? State { get; set; }




    }
}
