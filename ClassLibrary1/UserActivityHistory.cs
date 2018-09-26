using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstaBot.Common
{
    public class UserActivityHistory
    {
        [Key]
        public int Id { get; set; }
        public int QueueId { get; set; }
        [ForeignKey("QueueId")]
        public Queue Queue { get; set; }
        public DateTime CreatedOn { get; set; }
        public string PostedImageURI { get; set; }
    }
}
