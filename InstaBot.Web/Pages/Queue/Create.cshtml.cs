using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using InstaBot.Web.Data;
using InstaBot.Web.EntityModels;
using Microsoft.AspNetCore.Authorization;

namespace InstaBot.Web.Pages.Queue
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly InstaBot.Web.Data.ApplicationDbContext _context;

        public CreateModel(InstaBot.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["LoginDataId"] = new SelectList(_context.LoginData, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public QueueEntity QueueEntity { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Queues.Add(QueueEntity);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}