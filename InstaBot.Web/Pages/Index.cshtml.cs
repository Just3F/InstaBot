using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using InstaBot.Web.Data;
using InstaBot.Web.EntityModels;
using InstaBot.Web.Services;
using InstaBot.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstaBot.Web.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IQueueService _queueService;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _queueService = new QueueService(db);
            _userManager = userManager;
        }
        public IEnumerable<QueueEntity> Queues { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Queues = await _queueService.GetQueuesForUser(userId);
        }
    }
}
