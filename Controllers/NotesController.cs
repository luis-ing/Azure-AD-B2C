using App_Azure_OpenId.Models;
using App_Azure_OpenId.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App_Azure_OpenId.Controllers
{
	public class NotesController : Controller
	{
		private readonly NotesDbContext _context;

		public NotesController(NotesDbContext context)
		{
			_context = context;
		}
		public async Task<ActionResult> Index()
		{

			return View(await _context.Notes.ToListAsync());
		}

		public IActionResult Create()
		{
			//ViewData[]
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(NoteViewModel model)
		{
			if (ModelState.IsValid)
			{
				var note = new Note()
				{
					Title = model.Title,
					Content = model.Content,
					DateCreated = new DateTime(),
					UserCreated = 2,
					UserUpdated = 2,
				};
				_context.Notes.Add(note);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(model);
		}

	}
}
