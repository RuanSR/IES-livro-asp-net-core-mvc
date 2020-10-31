using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IES.Data;
using IES.Data.DAL.Cadastros;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;

namespace IES.Controllers
{
    public class DepartamentoController : Controller
    {
        private readonly IESContext _context;
        private readonly DepartamentoDAL departamentoDAL;
        private readonly InstituicaoDAL instituicaoDAL;

        public DepartamentoController(IESContext context)
        {
            _context = context;
            instituicaoDAL = new InstituicaoDAL(context);
            departamentoDAL = new DepartamentoDAL(context);
        }

        public async Task<IActionResult> Index()
        {
            return View(await departamentoDAL.ObterDepartamentosClassificadosPorNome().ToListAsync());
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
                    await departamentoDAL.GravarDepartamento(departamento);
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
                    await departamentoDAL.GravarDepartamento(departamento);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DepartamentoExists(departamento.DepartamentoID))
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
            return await ObterVisaoDepartamentoPorId(id);
        }

        public async Task<IActionResult> Delete(long? id)
        {
            return await ObterVisaoDepartamentoPorId(id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var departamento = await departamentoDAL.EliminarDepartamentoPorId((long)id);
            TempData["Message"] = "Departamento " + departamento.Nome.ToUpper() + " foi removido";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> ObterVisaoDepartamentoPorId(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await departamentoDAL.ObterDepartamentoPorId((long)id);
            if (departamento == null)
            {
                return NotFound();
            }

            return View(departamento);
        }

        private async Task<bool> DepartamentoExists(long? id)
        {
            return await departamentoDAL.ObterDepartamentoPorId((long)id) != null;
        }
    }
}
