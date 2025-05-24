// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Patrimonios.Data;
using Patrimonios.Services;

namespace Patrimonios.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<PatrimoniosUser> _signInManager;
        private readonly UserManager<PatrimoniosUser> _userManager;
        private readonly IUserStore<PatrimoniosUser> _userStore;
        private readonly IUserEmailStore<PatrimoniosUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly LogService _logService;

        public RegisterModel(
            UserManager<PatrimoniosUser> userManager,
            IUserStore<PatrimoniosUser> userStore,
            SignInManager<PatrimoniosUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            LogService logService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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
            [Required(ErrorMessage = "E-mail é obrigatório.")]
            [EmailAddress]
            [Display(Name = "E-mail")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Senha é obrigatório.")]
            [StringLength(20, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Senha de confirmação é obrigatório.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirme a senha")]
            [Compare("Password", ErrorMessage = "As senhas não são iguais.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logService.RegistrarLog(
                       Input.Email ?? "Usuário desconhecido",
                       "Registrar",
                       $"O usuário \"{user.UserName}\" criou uma nova conta."
                    );

                    _logger.LogInformation("O usuário criou uma nova conta com senha.");

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
                    <p>Olá,</p>
                    <p>Recemos uma solicitação para criar uma conta com este endereço de e-mail no <strong>Patrimônios</strong>.</p>
                    <br>
                    <p>Para confirmar, clique no link abaixo:</p>
                    <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Confirmar e-mail.</a></p>
                    <br>
                    <p>Se você não efetuou essa solicitação, nenhuma ação é necessária.</p>
                    <p>Atenciosamente,<br/>
                    Equipe Patrimônios</p>";

                    await _emailSender.SendEmailAsync(Input.Email, subject, body);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                    {
                        ModelState.AddModelError(string.Empty, "Este e-mail já está sendo usado. Faça login ou utilize outro e-mail.");
                    } else
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private PatrimoniosUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<PatrimoniosUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Não é possível criar uma instância de '{nameof(PatrimoniosUser)}'. " +
                    $"Garantir que '{nameof(PatrimoniosUser)}' não é uma classe abstrata e tem um construtor sem parâmetros, ou alternativamente " +
                    $"substituir a página de registro em /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<PatrimoniosUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("A interface de usuário padrão requer um armazenamento de usuário com suporte a e-mail.");
            }
            return (IUserEmailStore<PatrimoniosUser>)_userStore;
        }
    }
}
