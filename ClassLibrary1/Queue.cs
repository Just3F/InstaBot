using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InstaBot.Common.Enums;

namespace InstaBot.Common
{
    public class Queue
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public QueueType QueueType { get; set; }
        public QueueState QueueState { get; set; }
        public int DelayInSeconds { get; set; }
        public DateTime LastActivity { get; set; }
        public string LoadId { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
    }
}
