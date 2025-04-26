using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patrimonios.Data;
using Patrimonios.Models;
using Patrimonios.Services;

namespace Patrimonios.Controllers
{
    [Authorize]
    public class PatrimoniosController : Controller
    {
        private readonly PatrimoniosContext _context;
        private readonly LogService _logService;

        public PatrimoniosController(PatrimoniosContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public async Task<IActionResult> Index()
        {

            var patrimonios = await _context.Patrimonios.ToListAsync();
            return View(patrimonios);
        }

        public IActionResult Adicionar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Adicionar([Bind("Id,NumeroPatrimonio,Categoria,Equipamento,Marca,Modelo,Descricao,Localidade,Unidade,Setor,EstadoConservacao,Status,NotaFiscal,NumeroSerie,DataAquisicao,Valor")] PatrimoniosModel patrimonio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patrimonio);
                await _context.SaveChangesAsync();

                var emailUsuario = User.Identity?.Name;

                _logService.RegistrarLog(
                  emailUsuario,
                  "Adicionar",
                  $"O patrimônio Nº {patrimonio.NumeroPatrimonio} com a descrição: \"{patrimonio.Descricao}\" foi adicionado."
                );

                return RedirectToAction("Index");
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(patrimonio);
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patrimonio = await _context.Patrimonios.FindAsync(id);
            if (patrimonio == null)
            {
                return NotFound();
            }


            return View(patrimonio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("Id,NumeroPatrimonio,Categoria,Equipamento,Marca,Modelo,Descricao,Localidade,Unidade,Setor,EstadoConservacao,Status,NotaFiscal,NumeroSerie,DataAquisicao,Valor")] PatrimoniosModel patrimonio)
        {
            if (id != patrimonio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var patrimonioAtual = await _context.Patrimonios.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (patrimonioAtual == null)
                    {
                        return NotFound();
                    }

                    var alteracoes = new List<string>();

                    if (patrimonioAtual.NumeroPatrimonio != patrimonio.NumeroPatrimonio)
                    {
                        alteracoes.Add($"Número Patrimônio: {patrimonioAtual.NumeroPatrimonio} alterado para {patrimonio.NumeroPatrimonio}.");
                    }
                    if (patrimonioAtual.Categoria != patrimonio.Categoria)
                    {
                        alteracoes.Add($"Categoria: {patrimonioAtual.Categoria} alterado para {patrimonio.Categoria}.");
                    }
                    if (patrimonioAtual.Marca != patrimonio.Marca)
                    {
                        alteracoes.Add($"Marca: {patrimonioAtual.Marca} alterado para {patrimonio.Marca}.");
                    }
                    if (patrimonioAtual.Modelo != patrimonio.Modelo)
                    {
                        alteracoes.Add($"Modelo: {patrimonioAtual.Modelo} alterado para {patrimonio.Modelo}");
                    }
                    if (patrimonioAtual.Descricao != patrimonio.Descricao)
                    {
                        alteracoes.Add($"Descrição: {patrimonioAtual.Descricao} alterado para {patrimonio.Descricao}");
                    }
                    if (patrimonioAtual.Localidade != patrimonio.Localidade)
                    {
                        alteracoes.Add($"Localidade: {patrimonioAtual.Localidade} alterado para {patrimonio.Localidade}");
                    }
                    if (patrimonioAtual.Unidade != patrimonio.Unidade)
                    {
                        alteracoes.Add($"Unidade: {patrimonioAtual.Unidade} alterado para {patrimonio.Unidade}");
                    }
                    if (patrimonioAtual.Setor != patrimonio.Setor)
                    {
                        alteracoes.Add($"Setor: {patrimonioAtual.Setor} alterado para {patrimonio.Setor}");
                    }
                    if (patrimonioAtual.Status != patrimonio.Status)
                    {
                        alteracoes.Add($"Status: {patrimonioAtual.Status} alterado para {patrimonio.Status}");
                    }
                    if (patrimonioAtual.NotaFiscal != patrimonio.NotaFiscal)
                    {
                        alteracoes.Add($"Nota Fiscal: {patrimonioAtual.NotaFiscal} alterado para {patrimonio.NotaFiscal}");
                    }
                    if (patrimonioAtual.NumeroSerie != patrimonio.NumeroSerie)
                    {
                        alteracoes.Add($"Número Série: {patrimonioAtual.NumeroSerie} alterado para {patrimonio.NumeroSerie}");
                    }
                    if (patrimonioAtual.DataAquisicao != patrimonio.DataAquisicao)
                    {
                        alteracoes.Add($"Data de Aquisição: {patrimonioAtual.DataAquisicao} alterado para {patrimonio.DataAquisicao}");
                    }
                    if (patrimonioAtual.Valor != patrimonio.Valor)
                    {
                        alteracoes.Add($"Valor: {patrimonioAtual.Valor} alterado para {patrimonio.Valor}");
                    }

                    _context.Update(patrimonio);
                    await _context.SaveChangesAsync();

                    var emailUsuario = User.Identity?.Name;

                    if (alteracoes.Count > 0)
                    {
                        _logService.RegistrarLog(
                            emailUsuario,
                            "Editar",
                            $"Alterações realizadas no patrimônio ID {patrimonio.Id}: {string.Join("; ", alteracoes)}"
                        );
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatrimonioExists(patrimonio.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(patrimonio);
        }



        public async Task<IActionResult> Excluir(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patrimonio = await _context.Patrimonios.FirstOrDefaultAsync(m => m.Id == id);

            if (patrimonio == null)
            {
                return NotFound();
            }

            return View(patrimonio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirConfirmado(int id)
        {
            var emailUsuario = User.Identity?.Name;

            var patrimonio = await _context.Patrimonios.FindAsync(id);

            _context.Patrimonios.Remove(patrimonio);
            await _context.SaveChangesAsync();

            _logService.RegistrarLog(
              emailUsuario,
              "Excluir",
              $"O patrimônio Nº {patrimonio.NumeroPatrimonio} com a descrição: \"{patrimonio.Descricao}\" foi excluído."
           );

            return RedirectToAction(nameof(Index));
        }

        private bool PatrimonioExists(int id)
        {
            return _context.Patrimonios.Any(e => e.Id == id);
        }
    }
}
