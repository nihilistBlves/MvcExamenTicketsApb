using MvcExamenTicketsApb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcExamenTicketsApb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcExamenTicketsApb.Controllers {
    public class ManageController : Controller {
        private ServiceApiEmpresa service;
        public ManageController(ServiceApiEmpresa service) {
            this.service = service;
        }
        public IActionResult LogIn() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(string username, string password) {
            string token =
                await this.service.GetTokenAsync(username, password);
            if (token == null) {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }
            else {
                Usuario usuario =
                    await this.service.FindUsuarioAsync(token);
                ClaimsIdentity identity =
                    new ClaimsIdentity
                    (CookieAuthenticationDefaults.AuthenticationScheme
                    , ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.Name
                    , usuario.Username));
                identity.AddClaim(new Claim("TOKEN", token));
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme
                    , principal, new AuthenticationProperties {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });
                return RedirectToAction("Index", "Home");

            }
        }
        public async Task<IActionResult> LogOut() {
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("TOKEN");
            return RedirectToAction("Index", "Home");
        }
    }
}
