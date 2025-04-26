using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Patrimonios.Data;
using Patrimonios.Services;

namespace Patrimonios.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<PatrimoniosUser> _userManager;
        private readonly SignInManager<PatrimoniosUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private readonly LogService _logService;

        public DeletePersonalDataModel(
            UserManager<PatrimoniosUser> userManager,
            SignInManager<PatrimoniosUser> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            LogService logService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _logService = logService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não é possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não é possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Senha incorreta.");
                    return Page();
                }
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Ocorreu um erro inesperado ao excluir o usuário.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("Usuário com ID '{UserId}' excluiu sua conta.", userId);

            _logService.RegistrarLog(
                User.Identity?.Name ?? "Usuário desconhecido",
                "Conta",
                "Usuário apagou sua própria conta."
            );

            return Redirect("~/");
        }
    }
}
