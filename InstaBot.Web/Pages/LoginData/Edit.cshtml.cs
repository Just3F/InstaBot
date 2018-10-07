using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InstaBot.Web.Data;
using InstaBot.Web.EntityModels;

namespace InstaBot.Web.Pages.LoginData
{
    public class EditModel : PageModel
    {
        private readonly InstaBot.Web.Data.ApplicationDbContext _context;

        public EditModel(InstaBot.Web.Data.ApplicationDbContext context)
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
           ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(LoginDataEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoginDataEntityExists(LoginDataEntity.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool LoginDataEntityExists(int id)
        {
            return _context.LoginData.Any(e => e.Id == id);
        }
    }
}
