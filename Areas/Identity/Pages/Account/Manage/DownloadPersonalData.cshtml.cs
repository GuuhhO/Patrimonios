// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Patrimonios.Data;
using Patrimonios.Services;

namespace Patrimonios.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<PatrimoniosUser> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;
        private readonly LogService _logService;

        public DownloadPersonalDataModel(
            UserManager<PatrimoniosUser> userManager,
            ILogger<DownloadPersonalDataModel> logger,
            LogService logService)
        {
            _userManager = userManager;
            _logger = logger;
            _logService = logService;
        }

        public IActionResult OnGet()
        {
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não é possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("Usuário com ID '{UserId}' pediu seus dados pessoais.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(PatrimoniosUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

            Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");

            _logService.RegistrarLog(
                User.Identity?.Name ?? "Usuário desconhecido",
                "Conta",
                "Usuário fez download dos dados de sua conta."
            );

            return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");

        }
    }
}
