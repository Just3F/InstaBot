using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InstaBot.Web.Pages.Queue
{
    public class IndexModel : PageModel
    {
        private readonly InstaBot.Web.Data.ApplicationDbContext _context;

        public IndexModel(InstaBot.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Common.Queue> Queue { get;set; }

        public async Task OnGetAsync()
        {
            Queue = await _context.Queues
                .Include(q => q.User).ToListAsync();
        }
    }
}
