﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Patrimonios.Data;
using Patrimonios.Services;

namespace Patrimonios.Areas.Identity.Pages.Account
{
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly UserManager<PatrimoniosUser> _userManager;
        private readonly SignInManager<PatrimoniosUser> _signInManager;
        private readonly LogService _logService;

        public ConfirmEmailChangeModel(UserManager<PatrimoniosUser> userManager, SignInManager<PatrimoniosUser> signInManager, LogService logService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logService = logService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            var emailAntigo = User.Identity?.Name;

            if (userId == null || email == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Não é possível carregar o usuário com ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                StatusMessage = "Erro ao alterar e-mail.";
                return Page();
            }            

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                StatusMessage = "Erro ao alterar nome de usuário.";
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Obrigado por confirmar sua alteração de e-mail.";

            var emailNovo = User.Identity?.Name;

            _logService.RegistrarLog(
                User.Identity?.Name ?? "Usuário desconhecido",
                "Conta",
                $"Usuário alterou seu e-mail de \"{emailAntigo}\" para \"{emailNovo}\"."
            );

            return Page();
        }
    }
}
