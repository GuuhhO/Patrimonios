// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Patrimonios.Data;

namespace Patrimonios.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<PatrimoniosUser> _userManager;

        public ConfirmEmailModel(UserManager<PatrimoniosUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Não é possível carregar o usuário com ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (HttpContext.Request.Path.Value == "Identity/Account/Manage")
            {
                if (user.EmailConfirmed)
                {
                    StatusMessage = result.Succeeded ? "E-mail confirmado." : "Erro ao confirmar seu e-mail.";
                } else
                {
                    StatusMessage = result.Succeeded ? "Confirme seu e-mail." : "Erro ao confirmar seu e-mail.";
                }
                
            } else
            {
                StatusMessage = result.Succeeded ? "E-mail confirmado. Você já pode usar suas credênciais de acesso para entrar no sistema. Clique no botão abaixo para entrar." : "Erro ao confirmar seu e-mail.";
            }
            
            return Page();
        }
    }
}
