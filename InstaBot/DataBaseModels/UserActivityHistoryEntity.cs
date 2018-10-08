using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstaBot.Service.DataBaseModels
{
    public class UserActivityHistoryEntity
    {
        [Key]
        public int Id { get; set; }
        public int QueueId { get; set; }
        [ForeignKey("QueueId")]
        public QueueEntity QueueEntity { get; set; }
        public DateTime CreatedOn { get; set; }
        public string PostedImageURI { get; set; }
    }
}
