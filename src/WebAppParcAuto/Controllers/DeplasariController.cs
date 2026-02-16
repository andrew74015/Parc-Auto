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
    public class DeplasariController : Controller
    {
        private readonly ILogger<DeplasariController> _logger;

        private readonly AppDbContext _appDbContext;


        public DeplasariController(ILogger<DeplasariController> logger, AppDbContext appDbContext)
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
            var viewModel = new Models.Deplasari.ListaDeplasariViewModel
            {
                PageSize = pageSize <= 0 ? 10 : pageSize,
                PageNumber = pageNumber <= 0 ? 1 : pageNumber,
                SearchKey = searchKey,
                TotalRecords = await _appDbContext.Deplasari.CountAsync()
            };

            var listaDeplasariQueryable = _appDbContext.Deplasari.AsNoTracking().AsSingleQuery()
                .Include(i => i.Angajat)
                .Include(i => i.Masina)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
                listaDeplasariQueryable = listaDeplasariQueryable.Where(d =>
                    CharIndex.Udf(searchKey, d.Descriere) > 0 ||
                    CharIndex.Udf(searchKey, d.Motiv) > 0 ||
                    CharIndex.Udf(searchKey, d.Angajat.Nume) > 0 ||
                    CharIndex.Udf(searchKey, d.Angajat.Prenume) > 0 ||
                    CharIndex.Udf(searchKey, d.Masina.NumarDeInmatriculare) > 0
                );

            viewModel.TotalFilteredRecords = listaDeplasariQueryable.Count();

            viewModel.PageCount = viewModel.TotalFilteredRecords / viewModel.PageSize + (viewModel.TotalFilteredRecords % viewModel.PageSize != 0 ? 1 : 0);

            if (viewModel.PageNumber > viewModel.PageCount)
                viewModel.PageNumber = viewModel.PageCount;

            if (viewModel.PageNumber == 0)
                viewModel.PageNumber = 1;

            var listaDeplasari = listaDeplasariQueryable
                .OrderBy(a => a.DataPlecare)
                .Skip((viewModel.PageNumber - 1) * viewModel.PageSize)
                .Take(viewModel.PageSize);

            foreach (var deplasare in listaDeplasari)
                viewModel.Deplasari.Add(DeplasareDto.FromDbDeplasare(deplasare));

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Adaugare(string? returnUrl)
        {
            var viewModel = new Models.Deplasari.AdaugareViewModel()
            {
                ReturnUrl = returnUrl
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adaugare(DeplasareDto deplasareDto, string? returnUrl)
        {
            var viewModel = new Models.Deplasari.AdaugareViewModel()
            {
                Deplasare = deplasareDto,
                ReturnUrl = returnUrl
            };

            if (!ModelState.IsValid)
            {
                viewModel.ModelErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();

                return View(viewModel);
            }

            try
            {
                _appDbContext.Deplasari.Add(deplasareDto.ToDbDeplasare());

                var masina = await _appDbContext.Masini.FirstAsync(m => m.Id == deplasareDto.MasinaId);

                masina.NumarDeKilometri += deplasareDto.Distanta;

                _appDbContext.SaveChanges();

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Adaugare)}, Data:\n {JsonConvert.SerializeObject(deplasareDto, Formatting.Indented)}");

                viewModel.DetaliiEroare = $"{Messages.ErrorOnDbAdd} ({ex.Message})";

                return View(viewModel);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Editare(Guid id, string? returnUrl)
        {
            var viewModel = new Models.Deplasari.EditareViewModel()
            {
                ReturnUrl = returnUrl
            };

            var deplasare = await _appDbContext.Deplasari.AsNoTracking().AsSingleQuery()
                .Include(i => i.Angajat)
                .Include(i => i.Masina)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (deplasare == null)
                viewModel.DetaliiEroare = Messages.RecordNotFound;
            else
            {
                viewModel.Deplasare = DeplasareDto.FromDbDeplasare(deplasare);
                viewModel.Deplasare.Angajat = AngajatDto.FromDbAngajat(deplasare.Angajat);
                viewModel.Deplasare.Masina = MasinaDto.FromDbMasina(deplasare.Masina);
            }


            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editare(DeplasareDto deplasareDto, string? returnUrl)
        {
            var viewModel = new Models.Deplasari.EditareViewModel()
            {
                Deplasare = deplasareDto,
                ReturnUrl = returnUrl
            };

            if (!ModelState.IsValid)
            {
                viewModel.ModelErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();

                return View(viewModel);
            }

            var deplasare = await _appDbContext.Deplasari.AsSingleQuery()
                .Include(m => m.Masina)
                .FirstOrDefaultAsync(a => a.Id == deplasareDto.Id);

            if (deplasare == null)
            {
                viewModel.DetaliiEroare = Messages.RecordNotFound;

                return View(viewModel);
            }

            try
            {
                deplasareDto.UpdateDbDeplasare(deplasare);

                _appDbContext.SaveChanges();

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Editare)}, Data:\n {JsonConvert.SerializeObject(deplasareDto, Formatting.Indented)}");

                viewModel.DetaliiEroare = $"{Messages.ErrorOnDbEdit} ({ex.Message})";

                return View(viewModel);
            }
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Stergere(Guid id, string? returnUrl)
        {
            var viewModel = new Models.Deplasari.StergereViewModel()
            {
                ReturnUrl = returnUrl
            };

            var deplasare = await _appDbContext.Deplasari.AsNoTracking().AsSingleQuery()
                .Include(i => i.Angajat)
                .Include(i => i.Masina)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (deplasare == null)
                viewModel.DetaliiEroare = Messages.RecordNotFound;
            else
            {
                viewModel.Deplasare = DeplasareDto.FromDbDeplasare(deplasare);
                viewModel.Deplasare.Angajat = AngajatDto.FromDbAngajat(deplasare.Angajat);
                viewModel.Deplasare.Masina = MasinaDto.FromDbMasina(deplasare.Masina);
            }

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Stergere(DeplasareDto deplasareDto, string? returnUrl)
        {
            var viewModel = new Models.Deplasari.StergereViewModel()
            {
                Deplasare = deplasareDto,
                ReturnUrl = returnUrl
            };

            if (!ModelState.IsValid)
            {
                viewModel.ModelErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();

                return View(viewModel);
            }

            var deplasare = await _appDbContext.Deplasari.AsSingleQuery()
                .Include(m => m.Masina)
                .FirstOrDefaultAsync(a => a.Id == deplasareDto.Id);

            if (deplasare == null)
            {
                viewModel.DetaliiEroare = Messages.RecordNotFound;

                return View(viewModel);
            }

            try
            {
                _appDbContext.Deplasari.Remove(deplasare);

                var masina = await _appDbContext.Masini.FirstAsync(m => m.Id == deplasareDto.MasinaId);

                masina.NumarDeKilometri -= deplasareDto.Distanta;

                _appDbContext.SaveChanges();

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Stergere)}, Data:\n {JsonConvert.SerializeObject(deplasareDto, Formatting.Indented)}");

                viewModel.DetaliiEroare = $"{Messages.ErrorOnDbDelete} ({ex.Message})";

                return View(viewModel);
            }
        }
    }
}
