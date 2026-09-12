using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Domain.Entities;
using SistemaTramites.Domain.Seguridad;
using SistemaTramites.Infrastructure.Data;

namespace SistemaTramites.Web.Controllers;

/// <summary>Protocolo notarial: lo indexa Archivo y lo supervisa el Notario (RF-08).</summary>
[Authorize(Roles = RolesApp.ArchivoYSupervision)]
public class ProtocolosController : Controller
{
    private readonly AppDbContext _context;

    public ProtocolosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Protocolos
    public async Task<IActionResult> Index()
    {
        var items = _context.Protocolos.Include(x => x.Tramite);
        return View(await items.ToListAsync());
    }

    // GET: Protocolos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.Protocolos.Include(x => x.Tramite)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // GET: Protocolos/Create
    public IActionResult Create()
    {
        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View();
    }

    // POST: Protocolos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Protocolo model)
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

    // GET: Protocolos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var model = await _context.Protocolos.FindAsync(id);
        if (model == null) return NotFound();

        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View(model);
    }

    // POST: Protocolos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Protocolo model)
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
                if (!_context.Protocolos.Any(e => e.Id == model.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        return View(model);
    }

    // GET: Protocolos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.Protocolos.Include(x => x.Tramite)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // POST: Protocolos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Protocolos.FindAsync(id);
        if (item != null)
        {
            _context.Protocolos.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro eliminado.";
        }
        return RedirectToAction(nameof(Index));
    }
}
