using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Domain.Entities;
using SistemaTramites.Domain.Seguridad;
using SistemaTramites.Infrastructure.Data;

namespace SistemaTramites.Web.Controllers;

/// <summary>Agenda de citas: la maneja quien atiende al publico (RF-02).</summary>
[Authorize(Roles = RolesApp.MesaOperativa)]
public class CitasController : Controller
{
    private readonly AppDbContext _context;

    public CitasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Citas
    public async Task<IActionResult> Index()
    {
        var items = _context.Citas.Include(x => x.Tramite);
        return View(await items.ToListAsync());
    }

    // GET: Citas/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.Citas.Include(x => x.Tramite)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // GET: Citas/Create
    public IActionResult Create()
    {
        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View();
    }

    // POST: Citas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cita model)
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

    // GET: Citas/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var model = await _context.Citas.FindAsync(id);
        if (model == null) return NotFound();

        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View(model);
    }

    // POST: Citas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Cita model)
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
                if (!_context.Citas.Any(e => e.Id == model.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View(model);
    }

    // GET: Citas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.Citas.Include(x => x.Tramite)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // POST: Citas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Citas.FindAsync(id);
        if (item != null)
        {
            _context.Citas.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro eliminado.";
        }
        return RedirectToAction(nameof(Index));
    }
}
