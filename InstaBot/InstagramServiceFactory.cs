using InstaBot.Service.Enums;

namespace InstaBot.Service.Models
{
    public static class InstagramServiceFactory
    {
        public static IInstagramExecutor CreateExecutor(QueueType queueType)
        {
            IInstagramExecutor executor = null;

            switch (queueType)
            {
                case QueueType.LikePhoto:
                    executor = new LikeExecutor();
                    break;
                case QueueType.PostPhoto:
                    break;
                case QueueType.FollowUsers:
                    break;
                default:
                    break;
            }

            return executor;
        }
    }
}
