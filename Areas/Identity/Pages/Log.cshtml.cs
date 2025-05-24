using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Patrimonios.Data;
using Patrimonios.Models;

namespace Patrimonios.Areas.Identity.Pages
{
    public class LogModel : PageModel
    {
        private readonly PatrimoniosContext _context;
        private readonly UserManager<PatrimoniosUser> _userManager;

        public List<LogEntry> Logs { get; set; } = new List<LogEntry>();

        public LogModel(PatrimoniosContext context, UserManager<PatrimoniosUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Obt�m o ID do usu�rio logado
            var userId = _userManager.GetUserId(User);

            // Verifica se o usu�rio existe e pertence ao grupo permitido (Grupo == 1)
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == userId);
            if (usuario == null || usuario.Grupo != 1)
            {
                // Retorna "Forbid" se o usu�rio n�o tem permiss�o
                return Forbid();
            }

            // Carrega os logs do banco de dados, ordenando por data
            Logs = await _context.Logs
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();

            return Page();
        }

    }
}
