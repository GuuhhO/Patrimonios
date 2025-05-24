using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Patrimonios.Data;
using Patrimonios.Models;
using Patrimonios.Services;
using Patrimonios.Settings;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;

namespace Patrimonios.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly PatrimoniosContext _context;

        private readonly UserManager<PatrimoniosUser> _userManager;

        private readonly ILogger<HomeController> _logger;

        private readonly IEmailSender _emailSender;

        private readonly LogService _logService;

        private readonly SignInManager<PatrimoniosUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, PatrimoniosContext context, IEmailSender emailSender, UserManager<PatrimoniosUser> userManager, LogService logService, SignInManager<PatrimoniosUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
            _logService = logService;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var patrimonios = await _context.Patrimonios.ToListAsync();
            var culture = new CultureInfo("pt-BR");

            var totalValor = patrimonios
                .Where(p => !string.IsNullOrEmpty(p.Valor)) // Verifica se o valor não é nulo ou vazio
                .Sum(p => decimal.Parse(p.Valor, NumberStyles.Currency, culture));

            var totalCount = patrimonios.Count();

            // Formata o valor total
            ViewBag.TotalValor = totalValor.ToString("N", culture);
            ViewBag.TotalCount = totalCount;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> SendEmail()
        {
            await _emailSender.SendEmailAsync("igustahenrique@gmail.com", "Teste do disparo de e-mails Patrimonios", "Enviado com sucesso. Obrigado.");
            return View();
        }

        public async Task<IActionResult> TesteEmail()
        {
            try
            {
                await _emailSender.SendEmailAsync("igustahenrique@gmail.com", "Teste do disparo de e-mails Patrimonios", "Enviado com sucesso. Obrigado.");
                ViewBag.Message = "Email enviado com sucesso";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Erro ao enviar o email: {ex.Message}";
            }

            return View();
        }

        public async Task<IActionResult> Configuracoes()
        {
            var userId = _userManager.GetUserId(User);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == userId);


            if (usuario == null || usuario.Grupo != 1)
            {
                return Forbid();
            }

            var usuarios = await _context.Usuarios.Where(u => u.Id != userId).ToListAsync();

            return View(usuarios);
        }

        public async Task<IActionResult> EditarUsuario(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if(usuario == null)
            {
                return NotFound();
            }
            var model = new EditarUsuarioViewModel
            {
                Id = usuario.Id,
                Name = usuario.Name,
                UserName = usuario.UserName,
                PhoneNumber = usuario.PhoneNumber,
                Grupo = usuario.Grupo
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(string id, EditarUsuarioViewModel usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Name = usuario.Name;
                user.UserName = usuario.UserName;
                user.Email = usuario.UserName;
                user.PhoneNumber = usuario.PhoneNumber;
                user.Grupo = usuario.Grupo;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Usuário atualizado com sucesso.";

                    _logService.RegistrarLog(
                    User.Identity?.Name ?? "Usuário desconhecido",
                    "Editar",
                    $"Editou o usuario {user.UserName}.");

                    TempData["SuccessMessage"] = "Usuário atualizado com sucesso.";

                    return RedirectToAction(nameof(Configuracoes));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(usuario);
        }

        public async Task<IActionResult> ExcluirUsuario(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == id);

            if (usuarios == null)
            {
                return NotFound();
            }

            return View(usuarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirConfirmado(string id)
        {
            var usuarios = await _context.Usuarios.FindAsync(id);

            var emailUsuarioExcluido = usuarios.Email;

            _context.Usuarios.Remove(usuarios);
            await _context.SaveChangesAsync();
            
            _logService.RegistrarLog(
               User.Identity?.Name ?? "Usuário desconhecido",
               "Excluir",
               $"O usuario \"{emailUsuarioExcluido}\" foi excluido do sistema."
            );

            return RedirectToAction(nameof(Configuracoes));
        }

        [HttpGet]
        public IActionResult AdicionarUsuario()
        {
            return View();
        }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarUsuario(AdicionarUsuarioViewModel usuario, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    returnUrl ??= Url.Content("~/");
                    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                    var user = new PatrimoniosUser
                    {
                        Name = usuario.Name,
                        UserName = usuario.UserName,
                        Email = usuario.UserName,
                        PhoneNumber = usuario.PhoneNumber,
                        Grupo = usuario.Grupo,
                        PasswordHash = usuario.Password,
                        EmailConfirmed = false
                    };

                    var result = await _userManager.CreateAsync(user, user.PasswordHash);

                    if (result.Succeeded)
                    {
                        var emailUsuario = User.Identity?.Name;
                        _logService.RegistrarLog(
                          emailUsuario ?? "Usuário desconhecido",
                          "Adicionar",
                          $"{emailUsuario} adicionou o usuário {usuario.UserName} com função {(usuario.Grupo == 1 ? "Admin" : usuario.Grupo == 2 ? "Comum" : "Outro")} no sistema."
                        );

                        // Gerar token de confirmação de e-mail
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        var subject = "Confirme seu e-mail - Patrimônios";
                        var body = $@"
                        <p>Olá, {user.Name}</p>
                        <p>Recemos uma solicitação para criar uma conta com este endereço de e-mail no <strong>Patrimônios</strong>.</p>
                        <p>Para confirmar, clique no link abaixo:</p>
                        <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Confirmar e-mail.</a></p>
                        <br>
                        <p>Se você não efetuou essa solicitação, nenhuma ação é necessária.</p>
                        <p>Atenciosamente,<br/>
                        Equipe Patrimônios</p>";

                        await _emailSender.SendEmailAsync(usuario.Email, subject, body);

                        TempData["SuccessMessageAddUser"] = "Usuário adicionado com sucesso! Um e-mail de confirmação foi enviado.";
                        return RedirectToAction("Configuracoes");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                            Console.WriteLine($"Erro ao criar usuário: {error.Code} - {error.Description}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro ao adicionar o usuário.");
                    Console.WriteLine($"Erro: {ex.Message}");
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Erro: {modelError.ErrorMessage}");
                }
            }

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmarEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "E-mail confirmado com sucesso!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Falha ao confirmar e-mail.";
                return RedirectToAction("Index", "Home");
            }
        }


        private bool UsuarioExists(string id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
