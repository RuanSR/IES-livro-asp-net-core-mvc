using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IES.Data;
using IES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IES.Controllers
{
    public class DepartamentoController : Controller
    {
        private readonly IESContext _context;

        public DepartamentoController(IESContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var departamentos = await _context.Departamentos
                .Include(i => i.Instituicao)
                .OrderBy(d => d.Nome).ToListAsync();
            return View(departamentos);
        }

        public IActionResult Create()
        {
            var instituicoes = _context.Instituicoes.OrderBy(i => i.Nome).ToList();
            instituicoes.Insert(0, new Instituicao() { InstituicaoID = 0, Nome = "Selecione a instituição" });
            ViewBag.Instituicoes = instituicoes;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome", "InstituicaoId")] Departamento departamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(departamento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Não foi possível inserir os dados.");
            }
            return View(departamento);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamentos.SingleOrDefaultAsync(m => m.DepartamentoID == id);

            if(departamento == null)
            {
                return NotFound();//Cod 404.
            }
            ViewBag.Instituices = new SelectList(_context.Instituicoes.OrderBy(b => b.Nome), "InstituicaoID", "Nome", departamento.InstituicaoId);
            return View(departamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("DepartamentoID,Nome, InstituicaoID")] Departamento departamento)
        {
            if(id != departamento.DepartamentoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(departamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartamentoExists(departamento.DepartamentoID))
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
            ViewBag.Instituicoes = new SelectList(_context.Instituicoes.OrderBy(b => b.Nome), "InstituicaoId", "Nome", departamento.InstituicaoId);
            return View(departamento);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var departamento = await _context.Departamentos
                .SingleOrDefaultAsync(m => m.DepartamentoID == id);
            _context.Instituicoes.Where(i => departamento.InstituicaoId == i.InstituicaoID).Load();

            if (departamento == null)
            {
                return NotFound();
            }
            return View(departamento);
        }

        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var departamento = await _context.Departamentos.SingleOrDefaultAsync(m => m.DepartamentoID == id);
            _context.Instituicoes.Where(i => departamento.InstituicaoId == i.InstituicaoID).Load();
            if (departamento == null)
            {
                return NotFound();
            }
            return View(departamento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var departamento = await _context.Departamentos.SingleOrDefaultAsync(m => m.DepartamentoID == id);
            _context.Departamentos.Remove(departamento);
            TempData["Message"] = "Departamento " + departamento.Nome.ToUpper() + " foi removido";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartamentoExists(long? id)
        {
            return _context.Departamentos.Any(e => e.DepartamentoID == id);
        }
    }
}
