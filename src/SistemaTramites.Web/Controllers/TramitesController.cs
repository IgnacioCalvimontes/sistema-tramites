using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Domain.Entities;
using SistemaTramites.Domain.Seguridad;
using SistemaTramites.Infrastructure.Data;

namespace SistemaTramites.Web.Controllers;

/// <summary>Expediente de tramites: todo el personal consulta; solo la mesa operativa edita.</summary>
[Authorize(Roles = RolesApp.Personal)]
public class TramitesController : Controller
{
    private readonly AppDbContext _context;

    public TramitesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Tramites
    public async Task<IActionResult> Index()
    {
        var items = _context.Tramites.Include(x => x.Ciudadano).Include(x => x.TipoTramite).Include(x => x.Oficial);
        return View(await items.ToListAsync());
    }

    // GET: Tramites/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.Tramites.Include(x => x.Ciudadano).Include(x => x.TipoTramite).Include(x => x.Oficial)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // GET: Tramites/Create
    [Authorize(Roles = RolesApp.MesaOperativa)]
    public IActionResult Create()
    {
        ViewData["CiudadanoId"] = new SelectList(_context.Ciudadanos, "Id", "Nombre");
        ViewData["TipoTramiteId"] = new SelectList(_context.TiposTramite, "Id", "Nombre");
        ViewData["OficialId"] = new SelectList(_context.Usuarios, "Id", "Nombre");
        return View();
    }

    // POST: Tramites/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesApp.MesaOperativa)]
    public async Task<IActionResult> Create(Tramite model)
    {
        if (ModelState.IsValid)
        {
            _context.Add(model);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        ViewData["CiudadanoId"] = new SelectList(_context.Ciudadanos, "Id", "Nombre");
        ViewData["TipoTramiteId"] = new SelectList(_context.TiposTramite, "Id", "Nombre");
        ViewData["OficialId"] = new SelectList(_context.Usuarios, "Id", "Nombre");
        return View(model);
    }

    // GET: Tramites/Edit/5
    [Authorize(Roles = RolesApp.MesaOperativa)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var model = await _context.Tramites.FindAsync(id);
        if (model == null) return NotFound();

        ViewData["CiudadanoId"] = new SelectList(_context.Ciudadanos, "Id", "Nombre");
        ViewData["TipoTramiteId"] = new SelectList(_context.TiposTramite, "Id", "Nombre");
        ViewData["OficialId"] = new SelectList(_context.Usuarios, "Id", "Nombre");
        return View(model);
    }

    // POST: Tramites/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesApp.MesaOperativa)]
    public async Task<IActionResult> Edit(int id, Tramite model)
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
                if (!_context.Tramites.Any(e => e.Id == model.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["CiudadanoId"] = new SelectList(_context.Ciudadanos, "Id", "Nombre");
        ViewData["TipoTramiteId"] = new SelectList(_context.TiposTramite, "Id", "Nombre");
        ViewData["OficialId"] = new SelectList(_context.Usuarios, "Id", "Nombre");
        return View(model);
    }

    // GET: Tramites/Delete/5
    [Authorize(Roles = RolesApp.MesaOperativa)]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.Tramites.Include(x => x.Ciudadano).Include(x => x.TipoTramite).Include(x => x.Oficial)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // POST: Tramites/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesApp.MesaOperativa)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Tramites.FindAsync(id);
        if (item != null)
        {
            _context.Tramites.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro eliminado.";
        }
        return RedirectToAction(nameof(Index));
    }
}
