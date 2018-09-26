using InstaBot.Common.Enums;
using InstaBot.Service.InstagramExecutors;
using InstaSharper.API;

namespace InstaBot.Service
{
    public static class InstagramServiceFactory
    {
        public static IInstagramExecutor CreateExecutor(QueueType queueType, IInstaApi instaApi)
        {
            IInstagramExecutor executor = null;

            switch (queueType)
            {
                case QueueType.LikePhoto:
                    executor = new LikeExecutor(instaApi);
                    break;
                case QueueType.PostMedia:
                    executor = new PostMediaExecutor(instaApi);
                    break;
                case QueueType.FollowUsers:
                    executor = new FollowUserExecutor(instaApi);
                    break;
                default:
                    break;
            }

            return executor;
        }
    }
}
