using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SOFTITO_Project.Models
{
    public class User : IdentityUser
    {

        [StringLength(100, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(100)")]
        public override string UserName { get; set; } = "";
        [StringLength(700, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(700)")]
        public string Name { get; set; } = "";
        [EmailAddress]
        [StringLength(100, MinimumLength = 5)]
        [Column(TypeName = "varchar(100)")]
        public override string Email { get; set; } = "";
        [Phone]
        [StringLength(30)]
        [Column(TypeName = "varchar(30)")]
        public override string? PhoneNumber { get; set; }
        public DateTime RegisterDate { get; set; }
        public byte StateId { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("StateId")]
        public State? State { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
    }
}
