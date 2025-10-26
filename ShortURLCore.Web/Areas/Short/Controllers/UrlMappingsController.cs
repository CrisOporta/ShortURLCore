using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShortURLCore.Infrastructure.Data;
using ShortURLCore.Infrastructure.Repositories.IRepositories;
using ShortURLCore.Models;

namespace ShortURLCore.Web.Areas.Short.Controllers
{
    [Area("Short")]
    public class UrlMappingsController : Controller
    {
        private readonly IUnitOfWork _uow;

        public UrlMappingsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: Short/UrlMappings
        public async Task<IActionResult> Index()
        {
            var urlMappings = await _uow.UrlMappings.GetAllAsync();
            return View(urlMappings ?? new List<UrlMapping>());
        }

        // GET: Short/UrlMappings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urlMapping = await _uow.UrlMappings.GetAsync(x => x.Id == id);
            if (urlMapping == null)
            {
                return NotFound();
            }

            return View(urlMapping);
        }

        // GET: Short/UrlMappings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Short/UrlMappings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OriginalUrl,ShortCode,CreatedAt,UpdatedAt")] UrlMapping urlMapping)
        {
            if (ModelState.IsValid)
            {
                await _uow.UrlMappings.CreateAsync(urlMapping);
                await _uow.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(urlMapping);
        }

        // GET: Short/UrlMappings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urlMapping = await _uow.UrlMappings.GetAsync(x => x.Id == id);
            if (urlMapping == null)
            {
                return NotFound();
            }
            return View(urlMapping);
        }

        // POST: Short/UrlMappings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OriginalUrl,ShortCode,CreatedAt,UpdatedAt")] UrlMapping urlMapping)
        {
            if (id != urlMapping.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _uow.UrlMappings.Update(urlMapping);
                    await _uow.CommitAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UrlMappingExists(urlMapping.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(urlMapping);
        }

        // GET: Short/UrlMappings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urlMapping = await _uow.UrlMappings.GetAsync(x => x.Id == id);
            if (urlMapping == null)
            {
                return NotFound();
            }

            return View(urlMapping);
        }

        // POST: Short/UrlMappings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var urlMapping = await _uow.UrlMappings.GetAsync(x => x.Id == id);
            if (urlMapping != null)
            {
                _uow.UrlMappings.Remove(urlMapping);
                await _uow.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> UrlMappingExists(int id)
        {
            var exists = await _uow.UrlMappings.GetAsync(x => x.Id == id);
            return exists != null;
        }
    }
}
