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
    public class DeleteModel : PageModel
    {
        private readonly InstaBot.Web.Data.ApplicationDbContext _context;

        public DeleteModel(InstaBot.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginDataEntity LoginDataEntity { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LoginDataEntity = await _context.LoginData
                .Include(l => l.ApplicationUser).FirstOrDefaultAsync(m => m.Id == id);

            if (LoginDataEntity == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LoginDataEntity = await _context.LoginData.FindAsync(id);

            if (LoginDataEntity != null)
            {
                _context.LoginData.Remove(LoginDataEntity);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
