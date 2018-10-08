using System.ComponentModel.DataAnnotations;

namespace InstaBot.Service.DataBaseModels
{
    public class LoginDataEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
