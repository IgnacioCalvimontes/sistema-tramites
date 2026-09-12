using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Domain.Entities;
using SistemaTramites.Domain.Seguridad;
using SistemaTramites.Infrastructure.Data;

namespace SistemaTramites.Web.Controllers;

/// <summary>Aranceles (RF-03): todo el personal los consulta, solo Notario y Administrador cambian montos.</summary>
[Authorize(Roles = RolesApp.Personal)]
public class TiposTramiteController : Controller
{
    private readonly AppDbContext _context;

    public TiposTramiteController(AppDbContext context)
    {
        _context = context;
    }

    // GET: TiposTramite
    public async Task<IActionResult> Index()
    {
        var items = _context.TiposTramite;
        return View(await items.ToListAsync());
    }

    // GET: TiposTramite/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.TiposTramite
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // GET: TiposTramite/Create
    [Authorize(Roles = RolesApp.Supervision)]
    public IActionResult Create()
    {

        return View();
    }

    // POST: TiposTramite/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesApp.Supervision)]
    public async Task<IActionResult> Create(TipoTramite model)
    {
        if (ModelState.IsValid)
        {
            _context.Add(model);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // GET: TiposTramite/Edit/5
    [Authorize(Roles = RolesApp.Supervision)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var model = await _context.TiposTramite.FindAsync(id);
        if (model == null) return NotFound();


        return View(model);
    }

    // POST: TiposTramite/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesApp.Supervision)]
    public async Task<IActionResult> Edit(int id, TipoTramite model)
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
                if (!_context.TiposTramite.Any(e => e.Id == model.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // GET: TiposTramite/Delete/5
    [Authorize(Roles = RolesApp.Supervision)]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.TiposTramite
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // POST: TiposTramite/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesApp.Supervision)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.TiposTramite.FindAsync(id);
        if (item != null)
        {
            _context.TiposTramite.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro eliminado.";
        }
        return RedirectToAction(nameof(Index));
    }
}
