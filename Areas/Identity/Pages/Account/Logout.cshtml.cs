// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Patrimonios.Data;
using Patrimonios.Services;

namespace Patrimonios.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<PatrimoniosUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly LogService _logService;

        public LogoutModel(SignInManager<PatrimoniosUser> signInManager, ILogger<LogoutModel> logger, LogService logService)
        {
            _signInManager = signInManager;
            _logger = logger;
            _logService = logService;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Sair");

            _logService.RegistrarLog(
                User.Identity?.Name ?? "Usuário desconhecido",
                "Sair",
                "Usuário saiu do sistema."
            );

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
