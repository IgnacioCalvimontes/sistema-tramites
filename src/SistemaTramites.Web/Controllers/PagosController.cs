using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Domain.Entities;
using SistemaTramites.Infrastructure.Data;

namespace SistemaTramites.Web.Controllers;

public class PagosController : Controller
{
    private readonly AppDbContext _context;

    public PagosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Pagos
    public async Task<IActionResult> Index()
    {
        var items = _context.Pagos.Include(x => x.Tramite).Include(x => x.Cajero);
        return View(await items.ToListAsync());
    }

    // GET: Pagos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.Pagos.Include(x => x.Tramite).Include(x => x.Cajero)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // GET: Pagos/Create
    public IActionResult Create()
    {
        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        ViewData["CajeroId"] = new SelectList(_context.Usuarios, "Id", "Nombre");
        return View();
    }

    // POST: Pagos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Pago model)
    {
        if (ModelState.IsValid)
        {
            _context.Add(model);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        ViewData["CajeroId"] = new SelectList(_context.Usuarios, "Id", "Nombre");
        return View(model);
    }

    // GET: Pagos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var model = await _context.Pagos.FindAsync(id);
        if (model == null) return NotFound();

        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        ViewData["CajeroId"] = new SelectList(_context.Usuarios, "Id", "Nombre");
        return View(model);
    }

    // POST: Pagos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Pago model)
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
                if (!_context.Pagos.Any(e => e.Id == model.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["TramiteId"] = new SelectList(_context.Tramites, "Id", "CodigoSeguimiento");
        ViewData["CajeroId"] = new SelectList(_context.Usuarios, "Id", "Nombre");
        return View(model);
    }

    // GET: Pagos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var item = await _context.Pagos.Include(x => x.Tramite).Include(x => x.Cajero)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // POST: Pagos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Pagos.FindAsync(id);
        if (item != null)
        {
            _context.Pagos.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Registro eliminado.";
        }
        return RedirectToAction(nameof(Index));
    }
}
