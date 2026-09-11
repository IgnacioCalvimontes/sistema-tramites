using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Domain.Entities;
using SistemaTramites.Infrastructure.Data;

namespace SistemaTramites.Web.Controllers;

public class DocumentosController : Controller
{
    private readonly AppDbContext _context;

    public DocumentosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Documentos
    public async Task<IActionResult> Index()
    {
        var items = _context.Documentos.Include(x => x.Tramite);
        return View(await items.ToListAsync());
    }

    // GET: Documentos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.Documentos.Include(x => x.Tramite)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // GET: Documentos/Create
    public IActionResult Create()
    {
        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View();
    }

    // POST: Documentos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DocumentoAdjunto model)
    {
        if (ModelState.IsValid)
        {
            _context.Add(model);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View(model);
    }

    // GET: Documentos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var model = await _context.Documentos.FindAsync(id);
        if (model == null) return NotFound();

        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View(model);
    }

    // POST: Documentos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DocumentoAdjunto model)
    {
        if (id != model.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Registro actualizado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Documentos.Any(e => e.Id == model.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View(model);
    }

    // GET: Documentos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.Documentos.Include(x => x.Tramite)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // POST: Documentos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Documentos.FindAsync(id);
        if (item != null)
        {
            _context.Documentos.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro eliminado.";
        }
        return RedirectToAction(nameof(Index));
    }
}
