﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNotes.Data;
using MyNotes.Models.Entities;
using MyNotes.Models.ViewModels;

namespace MyNotes.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public NoteController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("{page:int?}")]
        public async Task<IActionResult> Index([FromRoute] int page = 1, int pageSize = 12)
        {
            var user = await _userManager.GetUserAsync(User);
            string userId = user!.Id;
            int countItemSkip = (page - 1) * pageSize;
            int totalItems = await _dbContext.Notes
                .Where(n => n.ApplicationUserId == userId)
                .CountAsync();

            var viewModel = new IndexNoteViewModel<NoteViewModel>();
            viewModel.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            viewModel.Items = await _dbContext.Notes
                .Where(n => n.ApplicationUserId == userId)
                .Skip(countItemSkip)
                .Take(pageSize)
                .Select(n => new NoteViewModel
                {
                    Id = n.Id,
                    Content = n.Content,
                    Title = n.Title
                })
                .ToListAsync();
            viewModel.PageIndex = page;
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] NoteViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var userId = user!.Id;
                Note note = new Note
                {
                    Title = model.Title,
                    Content = model.Content,
                    ApplicationUserId = userId
                };
                await _dbContext.Notes.AddAsync(note);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var note = await _dbContext.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            return View(new NoteViewModel
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] NoteViewModel model)
        {
            if(ModelState.IsValid)
            {
                var note = await _dbContext.Notes.FindAsync(model.Id);
                if(note == null)
                {
                    return NotFound();
                }
                note.Title = model.Title;
                note.Content = model.Content;
                _dbContext.Notes.Update(note);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var note = await _dbContext.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            return View(new NoteViewModel
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var note = await _dbContext.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            _dbContext.Notes.Remove(note);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
