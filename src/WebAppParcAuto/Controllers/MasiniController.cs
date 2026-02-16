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
    public class MasiniController : Controller
    {
        private readonly ILogger<MasiniController> _logger;

        private readonly AppDbContext _appDbContext;


        public MasiniController(ILogger<MasiniController> logger, AppDbContext appDbContext)
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
            var viewModel = new Models.Masini.ListaMasiniViewModel
            {
                PageSize = pageSize <= 0 ? 10 : pageSize,
                PageNumber = pageNumber <= 0 ? 1 : pageNumber,
                SearchKey = searchKey,
                TotalRecords = await _appDbContext.Masini.CountAsync()
            };

            var listaMasiniQueryable = _appDbContext.Masini.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
                listaMasiniQueryable = listaMasiniQueryable.Where(a =>
                    CharIndex.Udf(searchKey, a.Marca) > 0 ||
                    CharIndex.Udf(searchKey, a.Model) > 0 ||
                    CharIndex.Udf(searchKey, a.NumarDeInmatriculare) > 0
                );

            viewModel.TotalFilteredRecords = listaMasiniQueryable.Count();

            viewModel.PageCount = viewModel.TotalFilteredRecords / viewModel.PageSize + (viewModel.TotalFilteredRecords % viewModel.PageSize != 0 ? 1 : 0);

            if (viewModel.PageNumber > viewModel.PageCount)
                viewModel.PageNumber = viewModel.PageCount;

            if (viewModel.PageNumber == 0)
                viewModel.PageNumber = 1;

            var listaMasini = listaMasiniQueryable
                .OrderBy(a => a.Marca).ThenBy(a => a.Model)
                .Skip((viewModel.PageNumber - 1) * viewModel.PageSize)
                .Take(viewModel.PageSize);

            foreach (var masina in listaMasini)
            {
                viewModel.Masini.Add(MasinaDto.FromDbMasina(masina));
            }

            return View(viewModel);
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Adaugare(string? returnUrl)
        {
            var viewModel = new Models.Masini.AdaugareViewModel()
            {
                ReturnUrl = returnUrl
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Adaugare(MasinaDto masinaDto, string? returnUrl)
        {
            var viewModel = new Models.Masini.AdaugareViewModel()
            {
                Masina = masinaDto,
                ReturnUrl = returnUrl
            };

            if (!ModelState.IsValid)
            {
                viewModel.ModelErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();

                return View(viewModel);
            }

            try
            {
                _appDbContext.Masini.Add(masinaDto.ToDbMasina());

                _appDbContext.SaveChanges();

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Adaugare)}, Data:\n {JsonConvert.SerializeObject(masinaDto, Formatting.Indented)}");

                viewModel.DetaliiEroare = $"{Messages.ErrorOnDbAdd} ({ex.Message})";

                return View(viewModel);
            }
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Editare(Guid id, string? returnUrl)
        {
            var viewModel = new Models.Masini.EditareViewModel()
            {
                ReturnUrl = returnUrl
            };

            var masina = await _appDbContext.Masini.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

            if (masina == null)
                viewModel.DetaliiEroare = Messages.RecordNotFound;
            else
                viewModel.Masina = MasinaDto.FromDbMasina(masina);

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Editare(MasinaDto masinaDto, string? returnUrl)
        {
            var viewModel = new Models.Masini.EditareViewModel()
            {
                Masina = masinaDto,
                ReturnUrl = returnUrl
            };

            if (!ModelState.IsValid)
            {
                viewModel.ModelErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();

                return View(viewModel);
            }

            var masina = await _appDbContext.Masini.FirstOrDefaultAsync(a => a.Id == masinaDto.Id);

            if (masina == null)
            {
                viewModel.DetaliiEroare = Messages.RecordNotFound;

                return View(viewModel);
            }

            try
            {
                masinaDto.UpdateDbMasina(masina);

                _appDbContext.SaveChanges();

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Editare)}, Data:\n {JsonConvert.SerializeObject(masinaDto, Formatting.Indented)}");

                viewModel.DetaliiEroare = $"{Messages.ErrorOnDbEdit} ({ex.Message})";

                return View(viewModel);
            }
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Stergere(Guid id, string? returnUrl)
        {
            var viewModel = new Models.Masini.StergereViewModel()
            {
                ReturnUrl = returnUrl
            };

            var masina = await _appDbContext.Masini.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

            if (masina == null)
                viewModel.DetaliiEroare = Messages.RecordNotFound;
            else
                viewModel.Masina = MasinaDto.FromDbMasina(masina);

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Stergere(MasinaDto masinaDto, string? returnUrl)
        {
            var viewModel = new Models.Masini.StergereViewModel()
            {
                Masina = masinaDto,
                ReturnUrl = returnUrl
            };

            if (!ModelState.IsValid)
            {
                viewModel.ModelErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();

                return View(viewModel);
            }

            var masina = await _appDbContext.Masini.FirstOrDefaultAsync(a => a.Id == masinaDto.Id);

            if (masina == null)
            {
                viewModel.DetaliiEroare = Messages.RecordNotFound;

                return View(viewModel);
            }

            try
            {
                _appDbContext.Masini.Remove(masina);

                _appDbContext.SaveChanges();

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Stergere)}, Data:\n {JsonConvert.SerializeObject(masinaDto, Formatting.Indented)}");

                viewModel.DetaliiEroare = $"{Messages.ErrorOnDbDelete} ({ex.Message})";

                return View(viewModel);
            }
        }
    }
}
