using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InstaBot.Web.Data;
using InstaBot.Web.EntityModels;

namespace InstaBot.Web.Pages.Queue
{
    public class DeleteModel : PageModel
    {
        private readonly InstaBot.Web.Data.ApplicationDbContext _context;

        public DeleteModel(InstaBot.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public QueueEntity QueueEntity { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            QueueEntity = await _context.Queues
                .Include(q => q.LoginData).FirstOrDefaultAsync(m => m.Id == id);

            if (QueueEntity == null)
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

            QueueEntity = await _context.Queues.FindAsync(id);

            if (QueueEntity != null)
            {
                _context.Queues.Remove(QueueEntity);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
