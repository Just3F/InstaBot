using System;
using System.Threading;
using System.Threading.Tasks;
using InstaSharper.API;
using InstaSharper.Classes;

namespace InstaBot.Service
{
    class Program
    {
        #region Hidden

        private const string Username = "thinkoutsidethespace";
        private const string Password = "Gfhjkm63934710";

        #endregion

        static void Main(string[] args)
        {
            var instaService = new InstagramBotService { IsEnabled = true, DelayInMilliseconds = 1000 };

            new Thread(() => { instaService.Process(); }).Start();

            Console.ReadLine();
        }
    }
}
