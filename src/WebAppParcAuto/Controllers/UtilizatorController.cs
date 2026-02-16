using Common.Constants;
using Common.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAppParcAuto.Models;

namespace WebAppParcAuto.Controllers
{
    public class UtilizatorController : Controller
    {
        [AllowAnonymous]
        public IActionResult Conectare(string? schema, string returnUrl)
        {
            ViewBag.Schema = schema;
            ViewBag.ReturnUrl = returnUrl;

            if (schema == null)
                return View();

            if (schema.Equals(SchemeAutentificare.Google, StringComparison.CurrentCultureIgnoreCase))
                return RedirectToAction(nameof(ConectareGoogle), new
                {
                    returnUrl
                });

            else
                throw new ApplicationException("Invalid schema");
        }


        [Authorize(AuthenticationSchemes = SchemeAutentificare.Google)]
        public IActionResult ConectareGoogle(string returnUrl)
        {
            return Redirect(returnUrl ?? "/");
        }


        [AllowAnonymous]
        public async Task<IActionResult> Deconectare(string? schema)
        {
            if (User.GetIsAuthenticated())
            {
                schema = User.GetClaimValue(CustomClaimTypes.AuthScheme);

                if (!string.IsNullOrWhiteSpace(schema) && schema != SchemeAutentificare.Google)
                    await HttpContext.SignOutAsync(schema);

                await HttpContext.SignOutAsync();
            }

            return RedirectToAction(nameof(Deconectat), new { schema });
        }


        [AllowAnonymous]
        public IActionResult Deconectat(string? schema)
        {
            ViewBag.Schema = schema;

            return View();
        }


        [AllowAnonymous]
        public IActionResult AccesRestrictionat(string? schema, string? url, string? returnUrl, string? message)
        {
            var model = new ErrorVm
            {
                Schema = schema,
                Url = url ?? returnUrl,
                Title = "Acces restricționat",
                Message = message,
                TraceIdentifier = HttpContext.TraceIdentifier,
                ActivityId = Activity.Current?.Id
            };

            return View("Error", model);
        }
    }
}
