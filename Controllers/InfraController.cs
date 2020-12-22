﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IES.Models.Infra;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IES.Controllers
{
    [Authorize]
    public class InfraController : Controller
    {
        //É responsável	por	ações relacionadas ao gerenciamento	de usuário, como sua criação\\
        private readonly UserManager<UsuarioDaAplicacao> _userManager;
        //É responsável por registrar o acesso  do usuário à aplicação\\
        private readonly SignInManager<UsuarioDaAplicacao> _signInManager;
        //Responsável por registrar	mensagens de Log e exibi-las no console\\
        private readonly ILogger _logger;

        public InfraController(
            UserManager<UsuarioDaAplicacao> userManager, 
            SignInManager<UsuarioDaAplicacao> signInManager, 
            ILogger<InfraController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Acessar(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Acessar(AcessarViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Senha, model.LembrarDeMim, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuário Autenticado.");
                    return RedirectToLocal(returnUrl);
                }
            }
            ModelState.AddModelError(string.Empty, "Falha na tentativa de login.");
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegistrarNovoUsuario(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarNovoUsuario(RegistrarNovoUsuarioViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = new UsuarioDaAplicacao { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuário criou uma nova conta com senha.");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _signInManager.SignInAsync(user, isPersistent : false);
                    _logger.LogInformation("Usuário acessou com a conta criada.");

                    return RedirectToLocal(returnUrl);
                }
                AddErros(result);
            }
            return View(model);
        }

        public async Task<IActionResult> Sair()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuário	realizou logout.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private void AddErros(IdentityResult result)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
