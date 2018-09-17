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
        public async Task Execute(Queue queue, InstaBotContext db, IInstaApi api)
        {
            var allGroups = queue.LoadId.Split(' ');
            bool isMediaPosted = false;
            foreach (var group in allGroups)
            {
                var instaMediaList = await api.GetUserMediaAsync(group, PaginationParameters.MaxPagesToLoad(0));
                InstaMedia firstMedia = instaMediaList?.Value?.FirstOrDefault();

                if (firstMedia != null && firstMedia.MediaType == InstaMediaType.Image && !IsAlreadyPosted(firstMedia.InstaIdentifier, db))
                {
                    isMediaPosted = await PostMediaAsync(firstMedia, api);
                }

                if (isMediaPosted || group.Equals(allGroups.LastOrDefault()))
                {
                    await UpdateQueueLastActivityAsync(queue, db);
                    return;
                }
            }
        }

        private async Task<bool> PostMediaAsync(InstaMedia media, IInstaApi api)
        {
            MediaPost mediaPost = GetMediaPost(media);
            InstaImage[] instaImages = LoadImagesAsync(mediaPost.URICollection);

            var postResult = await api.UploadPhotosAlbumAsync(instaImages, mediaPost.Caption);
            //TODO delete all loaded images

            return postResult.Succeeded;
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
                    URI = new Uri(Path.GetFullPath(imageNameAndPath), UriKind.Absolute).LocalPath
                };
                instaImages.Add(instaImage);
            }

            return instaImages.ToArray();
        }

        private MediaPost GetMediaPost(InstaMedia media)
        {
            MediaPost mediaPost = new MediaPost { Caption = media.Caption.Text };
            if (IsMediaMultiPhotos(media))
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
            return !media.Carousel.Any();
        }

        private bool IsAlreadyPosted(string id, InstaBotContext db)
        {
            return db.UserActivityHistories.Any(x => x.PostedImageURI == id && x.Queue.QueueType == QueueType.PostMedia);
        }
    }
}
