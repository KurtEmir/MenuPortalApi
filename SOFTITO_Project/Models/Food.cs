using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SOFTITO_Project.Models
{
    public class Food
    {
        public int Id { get; set; }

        [StringLength(200, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(200)")]
        public string Category { get; set; } = "";

        [StringLength(200, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; } = "";

        [StringLength(500, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(500)")]
        public string Details { get; set; } = "";
        [Range(0,short.MaxValue)]
        public short Price { get; set; }

        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public RestaurantBranch? Branch { get; set; }

        public byte StateId { get; set; }
        [ForeignKey("StateId")]
        public State? State { get; set; }





    }
}
