using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InstaBot.Web.Data;
using InstaBot.Web.EntityModels;

namespace InstaBot.Web.Pages.LoginData
{
    public class IndexModel : PageModel
    {
        private readonly InstaBot.Web.Data.ApplicationDbContext _context;

        public IndexModel(InstaBot.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<LoginDataEntity> LoginDataEntity { get;set; }

        public async Task OnGetAsync()
        {
            LoginDataEntity = await _context.LoginData
                .Include(l => l.ApplicationUser).ToListAsync();
        }
    }
}
