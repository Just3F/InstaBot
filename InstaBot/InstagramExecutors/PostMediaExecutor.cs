using System;
using System.Collections.Generic;
using System.IO;
using InstaBot.Service.DataBaseModels;
using InstaBot.Service.Models;
using InstaSharper.API;
using InstaSharper.Classes;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InstaBot.Service.Enums;
using InstaSharper.Classes.Models;

namespace InstaBot.Service.InstagramExecutors
{
    public class PostMediaExecutor : BaseExecutor, IInstagramExecutor
    {
        public PostMediaExecutor(IInstaApi instaApi)
            : base(instaApi) { }

        public async Task Execute(Queue queue, InstaBotContext db)
        {
            var allGroups = queue.LoadId.Split(' ');
            bool isMediaPosted = false;
            foreach (var group in allGroups)
            {
                var instaMediaList = await _instaApi.GetUserMediaAsync(group, PaginationParameters.MaxPagesToLoad(0));
                InstaMedia firstMedia = instaMediaList?.Value?.FirstOrDefault();

                if (firstMedia != null && firstMedia.MediaType != InstaMediaType.Video && !IsAlreadyPosted(firstMedia.InstaIdentifier, queue.User.Name, db))
                {
                    //firstMedia.Caption.Text = "Caption test";
                    isMediaPosted = await PostMediaAsync(firstMedia);
                }

                if (isMediaPosted || group.Equals(allGroups.LastOrDefault()))
                {
                    await UpdateQueueLastActivityAsync(queue, db);
                    await AddFinishedQueuToHistory(firstMedia.InstaIdentifier, queue, db);
                    Console.WriteLine($"PostMediaExecutor for {queue.User.Name}");
                    return;
                }
            }
        }

        private async Task<bool> PostMediaAsync(InstaMedia media)
        {
            MediaPost mediaPost = GetMediaPost(media);
            InstaImage[] instaImages = LoadImagesAsync(mediaPost.URICollection);

            bool postResult = await UploadPostAsync(instaImages, mediaPost.Caption);
            //TODO delete all loaded images

            return postResult;
        }

        public async Task AddFinishedQueuToHistory(string postIdentifier, Queue queue, InstaBotContext db)
        {
            db.UserActivityHistories.Add(new UserActivityHistory
            {
                Queue = queue,
                CreatedOn = DateTime.UtcNow,
                PostedImageURI = postIdentifier
            });
            await db.SaveChangesAsync();
        }

        private async Task<bool> UploadPostAsync(InstaImage[] instaImages, string caption)
        {
            IResult<InstaMedia> result = null;

            if (instaImages.Count() == 1)
            {
                result = await _instaApi.UploadPhotoAsync(instaImages.FirstOrDefault(), caption);
            }
            else
            {
                result = await _instaApi.UploadPhotosAlbumAsync(instaImages, caption);
            }

            return result.Succeeded;
        }

        private InstaImage[] LoadImagesAsync(IEnumerable<string> uriCollection)
        {
            List<InstaImage> instaImages = new List<InstaImage>();
            foreach (var uri in uriCollection)
            {
                var guid = Guid.NewGuid();
                var imageNameAndPath = $"c:\\InstaBotImg\\{guid}.jpg";

                WebClient webClient = new WebClient();
                webClient.DownloadFile(uri, imageNameAndPath);

                var instaImage = new InstaImage
                {
                    Height = 1080,
                    Width = 1080,
                    URI = new Uri(Path.GetFullPath(imageNameAndPath), UriKind.Absolute).LocalPath,
                };
                instaImages.Add(instaImage);
            }

            return instaImages.ToArray();
        }

        private MediaPost GetMediaPost(InstaMedia media)
        {
            MediaPost mediaPost = new MediaPost { Caption = media.Caption.Text };
            if (media.IsMultiPost)
            {
                mediaPost.URICollection = media.Carousel.Select(x => x.Images?.FirstOrDefault()?.URI).ToList();
            }
            else
            {
                mediaPost.URICollection = new[] { media.Images.FirstOrDefault()?.URI };
            }

            return mediaPost;

        }

        private bool IsMediaMultiPhotos(InstaMedia media)
        {
            return media.Carousel != null && media.Carousel.Any();
        }

        private bool IsAlreadyPosted(string id, string userName, InstaBotContext db)
        {
            return db.UserActivityHistories.Any(x => x.PostedImageURI == id && x.Queue.QueueType == QueueType.PostMedia && x.Queue.User.Name == userName);
        }
    }
}
