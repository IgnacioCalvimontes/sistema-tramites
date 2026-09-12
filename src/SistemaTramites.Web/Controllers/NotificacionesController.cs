using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Domain.Entities;
using SistemaTramites.Domain.Seguridad;
using SistemaTramites.Infrastructure.Data;

namespace SistemaTramites.Web.Controllers;

/// <summary>Notificaciones al ciudadano: las emite la mesa operativa (RF-09).</summary>
[Authorize(Roles = RolesApp.MesaOperativa)]
public class NotificacionesController : Controller
{
    private readonly AppDbContext _context;

    public NotificacionesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Notificaciones
    public async Task<IActionResult> Index()
    {
        var items = _context.Notificaciones.Include(x => x.Tramite);
        return View(await items.ToListAsync());
    }

    // GET: Notificaciones/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.Notificaciones.Include(x => x.Tramite)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // GET: Notificaciones/Create
    public IActionResult Create()
    {
        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View();
    }

    // POST: Notificaciones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Notificacion model)
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

    // GET: Notificaciones/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var model = await _context.Notificaciones.FindAsync(id);
        if (model == null) return NotFound();

        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View(model);
    }

    // POST: Notificaciones/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Notificacion model)
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
                if (!_context.Notificaciones.Any(e => e.Id == model.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View(model);
    }

    // GET: Notificaciones/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.Notificaciones.Include(x => x.Tramite)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // POST: Notificaciones/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Notificaciones.FindAsync(id);
        if (item != null)
        {
            _context.Notificaciones.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro eliminado.";
        }
        return RedirectToAction(nameof(Index));
    }
}
