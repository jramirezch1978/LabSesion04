using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Security.Claims;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

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
        public new IActionResult SignOut()
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
        [Authorize]
        public IActionResult TokenInfo()
        {
            // Obtener información del token del contexto del usuario
            var tokenInfo = new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                AuthenticationType = User.Identity?.AuthenticationType,
                Name = User.Identity?.Name,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                Issuer = User.FindFirst("iss")?.Value,
                Audience = User.FindFirst("aud")?.Value,
                Subject = User.FindFirst("sub")?.Value,
                ExpiresAt = User.FindFirst("exp")?.Value,
                IssuedAt = User.FindFirst("iat")?.Value,
                NotBefore = User.FindFirst("nbf")?.Value,
                AuthTime = User.FindFirst("auth_time")?.Value,
                TokenUse = User.FindFirst("token_use")?.Value,
                ClientId = User.FindFirst("client_id")?.Value,
                Scope = User.FindFirst("scp")?.Value,
                TenantId = User.FindFirst("tid")?.Value,
                Version = User.FindFirst("ver")?.Value
            };

            return View(tokenInfo);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFullIdToken()
        {
            try
            {
                // Intentar obtener los tokens usando múltiples métodos
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var idToken = await HttpContext.GetTokenAsync("id_token");
                var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
                
                // Si no están disponibles, intentar obtenerlos de las propiedades de autenticación
                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(idToken))
                {
                    var authResult = await HttpContext.AuthenticateAsync();
                    if (authResult?.Properties?.Items != null)
                    {
                        authResult.Properties.Items.TryGetValue(".Token.access_token", out accessToken);
                        authResult.Properties.Items.TryGetValue(".Token.id_token", out idToken);
                        authResult.Properties.Items.TryGetValue(".Token.refresh_token", out refreshToken);
                    }
                }
                
                var tokenData = new
                {
                    AccessToken = accessToken,
                    IdToken = idToken,
                    RefreshToken = refreshToken,
                    HasAccessToken = !string.IsNullOrEmpty(accessToken),
                    HasIdToken = !string.IsNullOrEmpty(idToken),
                    HasRefreshToken = !string.IsNullOrEmpty(refreshToken),
                    TokenInfo = "Tokens obtenidos del contexto de autenticación",
                    AuthenticationScheme = HttpContext.User.Identity?.AuthenticationType,
                    IsAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false,
                    Debug = new
                    {
                        AvailableTokens = HttpContext.User.Claims.Select(c => c.Type).ToList(),
                        ClaimsCount = HttpContext.User.Claims.Count(),
                        AuthProperties = HttpContext.User.Identity?.AuthenticationType
                    }
                };

                return Json(tokenData);
            }
            catch (Exception ex)
            {
                return Json(new { 
                    Error = "Error al obtener tokens", 
                    Details = ex.Message,
                    Note = "Los tokens pueden no estar disponibles dependiendo de la configuración",
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult TestAuthentication()
        {
            var authenticationInfo = new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                UserName = User.Identity?.Name,
                AuthenticationType = User.Identity?.AuthenticationType,
                ClaimsCount = User.Claims.Count(),
                ServerTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                RemoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                RequestScheme = Request.Scheme,
                RequestHost = Request.Host.Value,
                RequestPath = Request.Path.Value,
                HasSessionId = !string.IsNullOrEmpty(HttpContext.Session.Id),
                SessionId = HttpContext.Session.Id,
                CookieCount = Request.Cookies.Count,
                HeaderCount = Request.Headers.Count
            };

            return View(authenticationInfo);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ClaimsAnalysis()
        {
            var claimsAnalysis = new
            {
                TotalClaims = User.Claims.Count(),
                ClaimsByType = User.Claims.GroupBy(c => c.Type).Select(g => new { Type = g.Key, Count = g.Count() }),
                UniqueClaims = User.Claims.Select(c => c.Type).Distinct().Count(),
                SecurityClaims = User.Claims.Where(c => c.Type.Contains("security") || c.Type.Contains("auth"))
                    .Select(c => new { Type = c.Type, Value = c.Value }).ToList(),
                PersonalClaims = User.Claims.Where(c => c.Type.Contains("name") || c.Type.Contains("email") || c.Type.Contains("given") || c.Type.Contains("family"))
                    .Select(c => new { Type = c.Type, Value = c.Value }).ToList(),
                RoleClaims = User.Claims.Where(c => c.Type.Contains("role") || c.Type.Contains("group"))
                    .Select(c => new { Type = c.Type, Value = c.Value }).ToList(),
                AzureAdClaims = User.Claims.Where(c => c.Type.StartsWith("http://schemas.microsoft.com/") || c.Type.StartsWith("http://schemas.xmlsoap.org/"))
                    .Select(c => new { Type = c.Type, Value = c.Value }).ToList(),
                CustomClaims = User.Claims.Where(c => !c.Type.StartsWith("http://"))
                    .Select(c => new { Type = c.Type, Value = c.Value }).ToList()
            };

            return View(claimsAnalysis);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
} 