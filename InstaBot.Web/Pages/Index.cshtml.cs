using System.Collections.Generic;
using System.Threading.Tasks;
using InstaBot.Common;
using InstaBot.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InstaBot.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IList<Common.Queue> Queues { get; set; }

        public async Task OnGetAsync()
        {
            Queues = await _dbContext.Queues.Include(x=>x.LoginData).ToListAsync();
        }
    }
}
