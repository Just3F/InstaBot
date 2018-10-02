using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using InstaBot.Common;
using InstaBot.Common.Enums;
using InstaBot.Service.DataBaseModels;
using InstaBot.Service.Models;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using InstaSharper.Logger;
using Microsoft.EntityFrameworkCore;

namespace InstaBot.Service
{
    public class InstaService
    {
        private IInstaApi _api;
        private string[] comments = { "Hey. Come on my page. There are many cool space pictures here!", "Great feed!", "Hey. I'm trying to collect only the best pictures on my page. Come to my page.", "This picture is lit!!", "Wow. Looks really cool :)" };

        public bool IsEnabled { get; set; }
        public int DelayInMilliseconds { get; set; }

        public async Task Process()
        {
            while (IsEnabled)
            {
                try
                {
                    await Executer();
                    Thread.Sleep(DelayInMilliseconds);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(DateTime.UtcNow);
                }

            }
        }

        private async Task Executer()
        {
            using (var db = new InstaBotContext())
            {
                var activeQueues = await db.Queues
                    .Where(x => x.LastActivity < DateTime.UtcNow - TimeSpan.FromSeconds(x.DelayInSeconds) &&
                                x.QueueState == QueueState.InProgress)
                    .Include(x => x.LoginData)
                    .ToListAsync();

                foreach (var queue in activeQueues)
                {
                    if (await LoginAsync(queue.LoginData.Name, queue.LoginData.Password))
                    {
                        switch (queue.QueueType)
                        {
                            case QueueType.PostMedia:
                                await ExecuteQueuePostPhotosAsync(queue, db);
                                break;
                            case QueueType.LikePhoto:
                                await ExecuteQueueLikePhotosAsync(queue, db);
                                break;
                            case QueueType.FollowUsers:
                                await ExecuteQueueFollowRecomendations(queue, db);
                                break;
                            default:
                                break;
                        }
                        Console.WriteLine(DateTime.UtcNow);
                    }
                    else
                    {
                        //queue.QueueState = QueueState.StoppedWithError;
                        queue.Notes = "LoginData can not Auth. Check Username and Password.";
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task ExecuteQueueFollowRecomendations(Queue queue, InstaBotContext db)
        {
            var top = await _api.GetExploreFeedAsync(PaginationParameters.MaxPagesToLoad(0));
            var firstRecomendation = top.Value.Medias.FirstOrDefault();

            await _api.FollowUserAsync(firstRecomendation.User.Pk);

            Console.WriteLine("Followed for user - " + firstRecomendation.User.Pk);
            queue.LastActivity = DateTime.UtcNow;
            await db.SaveChangesAsync();
            Thread.Sleep(1000);
        }

        private async Task ExecuteQueueLikePhotosAsync(Queue queue, InstaBotContext db)
        {
            var tags = queue.LoadId.Split(' ');
            foreach (var tag in tags)
            {
                var top = await _api.GetExploreFeedAsync(PaginationParameters.MaxPagesToLoad(0));
                var media = top.Value.Medias.FirstOrDefault();

                //var foundPhotos = await _api.GetTagFeedAsync(tag, PaginationParameters.MaxPagesToLoad(0));
                //var media = foundPhotos.Value.Medias.FirstOrDefault();

                var isPhotoAlreadyLiked = db.UserActivityHistories.Any(x =>
                x.PostedImageURI == media.InstaIdentifier && x.Queue.QueueType == QueueType.LikePhoto);

                if (isPhotoAlreadyLiked)
                    break;

                await _api.LikeMediaAsync(media.InstaIdentifier);
                var rand = new Random();
                var isNeedToComment = rand.Next(3);
                if (isNeedToComment == 1)
                {
                    var randNumber = rand.Next(0, comments.Length);
                    var instaComment = await _api.CommentMediaAsync(media.InstaIdentifier, comments[randNumber]);
                    Console.WriteLine("Photo liked and commented!" + Environment.NewLine + instaComment.Info);
                }
                else
                {
                    Console.WriteLine("Photo liked only!");
                }


                queue.LastActivity = DateTime.UtcNow;
                db.UserActivityHistories.Add(new UserActivityHistory
                {
                    Queue = queue,
                    CreatedOn = DateTime.UtcNow,
                    PostedImageURI = media.InstaIdentifier,
                });
                await db.SaveChangesAsync();

                Thread.Sleep(1000);
            }
        }

        private async Task ExecuteQueuePostPhotosAsync(Queue queue, InstaBotContext db)
        {
            var allGroups = queue.LoadId.Split(' ');
            foreach (var group in allGroups)
            {
                var photosForPost = await _api.GetUserMediaAsync(group, PaginationParameters.MaxPagesToLoad(0));
                if (photosForPost.Value != null)
                {
                    var firstPhotoForPost = photosForPost.Value.FirstOrDefault();

                    if (firstPhotoForPost.MediaType == InstaMediaType.Video)
                        continue;

                    PhotoPost photoPost = new PhotoPost { Caption = firstPhotoForPost.Caption?.Text };
                    if (firstPhotoForPost.Images.Any())
                    {
                        photoPost.PhotoURI = firstPhotoForPost.Images.FirstOrDefault().URI;
                    }
                    else
                    {
                        photoPost.PhotoURI = firstPhotoForPost.Carousel.FirstOrDefault().Images.FirstOrDefault().URI;
                    }

                    var isPhotoPosted = await PostPhotoAsync(queue, db, photoPost);

                    if (isPhotoPosted)
                    {
                        break;
                    }
                    else
                    {
                        if (allGroups.LastOrDefault() == group)
                        {
                            queue.LastActivity = DateTime.UtcNow;
                            db.UserActivityHistories.Add(new UserActivityHistory
                            {
                                Queue = queue,
                                CreatedOn = DateTime.UtcNow
                            });
                            await db.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    if (allGroups.LastOrDefault() == group)
                    {
                        queue.LastActivity = DateTime.UtcNow;
                        db.UserActivityHistories.Add(new UserActivityHistory
                        {
                            Queue = queue,
                            CreatedOn = DateTime.UtcNow
                        });
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task<bool> PostPhotoAsync(Queue queue, InstaBotContext db, PhotoPost photoPost)
        {
            var imageURI = photoPost.PhotoURI;
            var success = false;
            var isPhotoAlreadyPosted = db.UserActivityHistories.Any(x =>
                x.PostedImageURI == imageURI && x.Queue.QueueType == QueueType.PostMedia);

            if (!isPhotoAlreadyPosted)
            {
                await UploadPhotoAsync(photoPost.Caption, imageURI);

                queue.LastActivity = DateTime.UtcNow;
                db.UserActivityHistories.Add(new UserActivityHistory
                {
                    Queue = queue,
                    CreatedOn = DateTime.UtcNow,
                    PostedImageURI = imageURI,
                });
                await db.SaveChangesAsync();

                success = true;
            }

            return success;
        }

        private async Task UploadPhotoAsync(string caption, string imageURI)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile(imageURI, @"c:\test.jpg");

            var mediaImage = new InstaImage
            {
                Height = 1080,
                Width = 1080,
                URI = new Uri(Path.GetFullPath(@"c:\test.jpg"), UriKind.Absolute).LocalPath
            };
            await _api.UploadPhotoAsync(mediaImage, caption);
            Console.WriteLine("Photo posted!");
        }

        private async Task<bool> LoginAsync(string username, string password)
        {
            if (_api != null && _api.IsUserAuthenticated)
                return true;
            var userSession = new UserSessionData {UserName = username, Password = password};
            _api = InstaApiBuilder.CreateBuilder()
                .SetUser(userSession)
                .UseLogger(new DebugLogger(LogLevel.Exceptions)).Build();

            var loginRequest = await _api.LoginAsync();
            if (loginRequest.Succeeded)
                Console.WriteLine("Logged in!");
            else
            {
                var errorMsg = loginRequest.Info.Message;
                Console.WriteLine("Error Logging in!" + Environment.NewLine + errorMsg);
                if (errorMsg == "Please wait a few minutes before you try again.")
                    Thread.Sleep(TimeSpan.FromMinutes(5));
            }

            return loginRequest.Succeeded;
        }
    }
}
