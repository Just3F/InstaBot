using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstaBot.Common
{
    public class LoginData
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        //[ForeignKey("UserId")]
        //public ApplicationUser User { get; set; }
        //[Column("UserId")]
        //public string UserId { get; set; }
    }
}
