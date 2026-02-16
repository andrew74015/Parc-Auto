using Common.Constants;
using Database.DataModels;
using Database.ScalarMappedFunctions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebAppParcAuto.Models;

namespace WebAppParcAuto.Controllers
{
    [Authorize(Roles = "Angajat,Administrator")]
    public class AngajatiController : Controller
    {
        private readonly ILogger<AngajatiController> _logger;

        private readonly AppDbContext _appDbContext;


        public AngajatiController(ILogger<AngajatiController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;            
        }


        public IActionResult Index()
        {
            return RedirectToAction(nameof(Lista));
        }


        public async Task<IActionResult> Lista(
            [FromQuery(Name = "pn")] int pageNumber,
            [FromQuery(Name = "ps")] int pageSize,
            [FromQuery(Name = "sk")] string? searchKey)
        {
            var viewModel = new Models.Angajati.ListaAngajatiViewModel
            {
                PageSize = pageSize <= 0 ? 10 : pageSize,
                PageNumber = pageNumber <= 0 ? 1 : pageNumber,
                SearchKey = searchKey,
                TotalRecords = await _appDbContext.Angajati.CountAsync()
            };

            var listaAngajatiQueryable = _appDbContext.Angajati.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
                listaAngajatiQueryable = listaAngajatiQueryable.Where(a =>
                    CharIndex.Udf(searchKey, a.Nume) > 0 ||
                    CharIndex.Udf(searchKey, a.Prenume) > 0 ||
                    CharIndex.Udf(searchKey, a.Email) > 0
                );

            viewModel.TotalFilteredRecords = listaAngajatiQueryable.Count();

            viewModel.PageCount = viewModel.TotalFilteredRecords / viewModel.PageSize + (viewModel.TotalFilteredRecords % viewModel.PageSize != 0 ? 1 : 0);
           
            if (viewModel.PageNumber > viewModel.PageCount)
                viewModel.PageNumber = viewModel.PageCount;

            if (viewModel.PageNumber == 0)
                viewModel.PageNumber = 1;

            var listaAngajati = listaAngajatiQueryable
                .OrderBy(a => a.Nume).ThenBy(a => a.Prenume)
                .Skip((viewModel.PageNumber - 1) * viewModel.PageSize)
                .Take(viewModel.PageSize);
                
            foreach (var angajat in listaAngajati)
            {
                viewModel.Angajati.Add(AngajatDto.FromDbAngajat(angajat));
            }

            return View(viewModel);
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Adaugare(string? returnUrl)
        {
            var viewModel = new Models.Angajati.AdaugareViewModel()
            {
                ReturnUrl = returnUrl
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Adaugare(AngajatDto angajatDto, string? returnUrl)
        {
            var viewModel = new Models.Angajati.AdaugareViewModel()
            {
                Angajat = angajatDto,
                ReturnUrl = returnUrl
            };

            if (!ModelState.IsValid)
            {
                viewModel.ModelErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();

                return View(viewModel);
            }

            try
            {
                _appDbContext.Angajati.Add(angajatDto.ToDbAngajat());

                _appDbContext.SaveChanges();

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Adaugare)}, Data:\n {JsonConvert.SerializeObject(angajatDto, Formatting.Indented)}");

                viewModel.DetaliiEroare = $"{Messages.ErrorOnDbAdd} ({ex.Message})";

                return View(viewModel);
            }
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Editare(Guid id, string? returnUrl)
        {
            var viewModel = new Models.Angajati.EditareViewModel()
            {
                ReturnUrl = returnUrl
            };

            var angajat = await _appDbContext.Angajati.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

            if (angajat == null)
                viewModel.DetaliiEroare = Messages.RecordNotFound;
            else
                viewModel.Angajat = AngajatDto.FromDbAngajat(angajat);

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Editare(AngajatDto angajatDto, string? returnUrl)
        {
            var viewModel = new Models.Angajati.EditareViewModel()
            {
                Angajat = angajatDto,
                ReturnUrl = returnUrl
            };

            if (!ModelState.IsValid)
            {                
                viewModel.ModelErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                
                return View(viewModel);
            }

            var angajat = await _appDbContext.Angajati.FirstOrDefaultAsync(a => a.Id == angajatDto.Id);

            if (angajat == null)
            {
                viewModel.DetaliiEroare = Messages.RecordNotFound;

                return View(viewModel);
            }                

            try
            {
                angajatDto.UpdateDbAngajat(angajat);

                _appDbContext.SaveChanges();

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Editare)}, Data:\n {JsonConvert.SerializeObject(angajatDto, Formatting.Indented)}");

                viewModel.DetaliiEroare = $"{Messages.ErrorOnDbEdit} ({ex.Message})";
                
                return View(viewModel);
            }
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Stergere(Guid id, string? returnUrl)
        {
            var viewModel = new Models.Angajati.StergereViewModel()
            {
                ReturnUrl = returnUrl
            };

            var angajat = await _appDbContext.Angajati.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

            if (angajat == null)
                viewModel.DetaliiEroare = Messages.RecordNotFound;
            else
                viewModel.Angajat = AngajatDto.FromDbAngajat(angajat);

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Stergere(AngajatDto angajatDto, string? returnUrl)
        {
            var viewModel = new Models.Angajati.StergereViewModel()
            {
                Angajat = angajatDto,
                ReturnUrl = returnUrl
            };

            if (!ModelState.IsValid)
            {
                viewModel.ModelErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();

                return View(viewModel);
            }

            var angajat = await _appDbContext.Angajati.FirstOrDefaultAsync(a => a.Id == angajatDto.Id);

            if (angajat == null)
            {
                viewModel.DetaliiEroare = Messages.RecordNotFound;

                return View(viewModel);
            }

            try
            {
                _appDbContext.Angajati.Remove(angajat);

                _appDbContext.SaveChanges();

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Stergere)}, Data:\n {JsonConvert.SerializeObject(angajatDto, Formatting.Indented)}");

                viewModel.DetaliiEroare = $"{Messages.ErrorOnDbDelete} ({ex.Message})";

                return View(viewModel);
            }
        }
    }
}
