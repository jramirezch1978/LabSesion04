using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace DevSeguroWebApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignIn()
        {
            var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [Authorize]
        public IActionResult SignOut()
        {
            return SignOut(
                new AuthenticationProperties 
                { 
                    RedirectUri = Url.Action(nameof(SignedOut), "Account") 
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignedOut()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Método para obtener información de tokens (solo desarrollo)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> TokenInfo()
        {
            if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                return BadRequest("Solo disponible en desarrollo");
            }

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var model = new
            {
                User = new
                {
                    Name = User.Identity?.Name,
                    IsAuthenticated = User.Identity?.IsAuthenticated,
                    AuthenticationType = User.Identity?.AuthenticationType,
                    Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
                },
                Tokens = new
                {
                    AccessTokenPresent = !string.IsNullOrEmpty(accessToken),
                    IdTokenPresent = !string.IsNullOrEmpty(idToken),
                    RefreshTokenPresent = !string.IsNullOrEmpty(refreshToken),
                    AccessTokenPreview = accessToken?.Substring(0, Math.Min(50, accessToken.Length)) + "...",
                    IdTokenPreview = idToken?.Substring(0, Math.Min(50, idToken.Length)) + "..."
                }
            };

            return Json(model);
        }

        // Método para obtener ID Token completo (solo desarrollo)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFullIdToken()
        {
            if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                return BadRequest("Solo disponible en desarrollo");
            }
            
            var idToken = await HttpContext.GetTokenAsync("id_token");
            return Json(new { IdToken = idToken });
        }
    }
} 