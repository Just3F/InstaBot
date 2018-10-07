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

namespace InstaBot.Web.Pages.Queue
{
    public class EditModel : PageModel
    {
        private readonly InstaBot.Web.Data.ApplicationDbContext _context;

        public EditModel(InstaBot.Web.Data.ApplicationDbContext context)
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
            ViewData["LoginDataId"] = new SelectList(_context.LoginData, "Id", "Id");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var queueEntity = _context.Queues.Single(x => x.Id == QueueEntity.Id);
            queueEntity.QueueType = QueueEntity.QueueType;
            queueEntity.QueueState = QueueEntity.QueueState;
            queueEntity.DelayInSeconds = QueueEntity.DelayInSeconds;
            queueEntity.LastActivity = QueueEntity.LastActivity;
            queueEntity.LoadId = QueueEntity.LoadId;
            queueEntity.IsActive = QueueEntity.IsActive;
            queueEntity.Notes = QueueEntity.Notes;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QueueEntityExists(QueueEntity.Id))
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

        private bool QueueEntityExists(int id)
        {
            return _context.Queues.Any(e => e.Id == id);
        }
    }
}
