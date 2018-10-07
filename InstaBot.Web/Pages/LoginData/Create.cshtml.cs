using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using InstaBot.Web.Data;
using InstaBot.Web.EntityModels;

namespace InstaBot.Web.Pages.LoginData
{
    public class CreateModel : PageModel
    {
        private readonly InstaBot.Web.Data.ApplicationDbContext _context;

        public CreateModel(InstaBot.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public LoginDataEntity LoginDataEntity { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.LoginData.Add(LoginDataEntity);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}