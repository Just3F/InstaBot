using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InstaBot.Service.Enums;

namespace InstaBot.Service.DataBaseModels
{
    public class QueueEntity
    {
        [Key]
        public int Id { get; set; }
        public QueueType QueueType { get; set; }
        public QueueState QueueState { get; set; }
        public int DelayInSeconds { get; set; }
        public DateTime LastActivity { get; set; }
        public string LoadId { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }

        public int LoginDataId { get; set; }
        [ForeignKey("LoginDataId")]
        public LoginDataEntity LoginData { get; set; }
    }
}
