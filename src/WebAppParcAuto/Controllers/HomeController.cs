using Common.Constants;
using Common.Extensions;
using Database.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebAppParcAuto.Models;

namespace WebAppParcAuto.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Dictionary<string, string[]> _marciSiModeleRandom = new()
        {
                {"BMW", ["M3", "M4", "M5"]},
                {"Mazda", ["CX-30", "CX-5", "CX-80"]},
                {"Dacia", ["Logan", "Duster", "Sandero"]},
                {"Renault", ["Clio", "Megane", "Captur"]},
                {"Toyota", ["Corolla", "Camry", "RAV4"]},
                {"Ford", ["Mustang", "F-150", "Explorer"]},
                {"Skoda", ["Octavia", "Superb", "Kodiaq"]},
                {"Opel", ["Corsa", "Astra", "Insignia"]},
                {"Mercedes-Benz", ["C-Class", "E-Class", "S-Class"]},
                {"Volkswagen", ["Golf", "Passat", "Tiguan"]},
                {"Audi", ["A3", "A4", "A6"]}
            };

        private static readonly Dictionary<string, string[]> _numeSiPrenumeRandom = new()
        {
                {"Popescu", new string[] { "Ion", "Maria", "George", "Elena", "Florin", "Daniela", "Mircea", "Cătălina", "Vlad", "Sorina" }},
                {"Ionescu", new string[] { "Ana", "Vasile", "Elena", "Mihai", "Carmen", "Cristian", "Andrei", "Daniela", "Radu", "Iulia" }},
                {"Georgescu", new string[] { "Dan", "Cristina", "Alin", "Andreea", "Ioan", "Gabriela", "Violeta", "Victor", "Anca", "Paul" }},
                {"Marin", new string[] { "Alexandru", "Simona", "Radu", "Roxana", "Gabriel", "Monica", "Teodor", "Diana", "Claudiu", "Eliza" }},
                {"Dumitrescu", new string[] { "Laura", "Sorin", "Liliana", "Victor", "Adriana", "Marius", "Ioana", "Florentin", "Cătălin", "Irina" }},
                {"Radulescu", new string[] { "Adrian", "Nicoleta", "Sergiu", "Marina", "Bogdan", "Anca", "Cosmin", "Roxana", "Felix", "Alexandra" }},
                {"Stoian", new string[] { "Roxana", "Andrei", "Irina", "Tudor", "Alexandra", "Mihail", "Doru", "Bianca", "Octavian", "Olga" }},
                {"Iliescu", new string[] { "Lucian", "Lavinia", "Cosmin", "Bianca", "Stefan", "Oana", "Marian", "Camelia", "Valeriu", "Raluca" }},
                {"Petrescu", new string[] { "Emil", "Claudia", "Sebastian", "Alina", "Valentin", "Mihaela", "Ionuț", "Veronica", "Cristian", "Gabriela" }},
                {"Tudor", new string[] { "Nicolae", "Diana", "Alex", "Sofia", "Rares", "Georgiana", "Sergiu", "Ana", "Ilinca", "Florin" }},
                {"Stan", new string[] { "Gabriel", "Anca", "Marius", "Daria", "Iulian", "Ruxandra", "Nicoleta", "Daniel", "Florin", "Cristiana" }},
                {"Stoica", new string[] { "Oana", "George", "Ileana", "Alexandru", "Simona", "Constantin", "Madalina", "Cezar", "Denisa", "Marian" }},
                {"Antonescu", new string[] { "Laura", "Victor", "Adriana", "Cristian", "Ioana", "Stefan", "Andreea", "Cosmin", "Eliza", "Bogdan" }},
                {"Constantinescu", new string[] { "Andrei", "Marina", "Florin", "Gabriela", "Ioan", "Claudia", "Liviu", "Ana", "Alexandru", "Diana" }},
                {"Dobre", new string[] { "Valentin", "Elena", "Sorin", "Daniela", "Mihai", "Cristina", "Roxana", "Alexandru", "Iulia", "George" }},
                {"Grigorescu", new string[] { "Alexandra", "Cristian", "Emilia", "Bogdan", "Marian", "Eliza", "Ioan", "Adela", "Răzvan", "Loredana" }},
                {"Lazar", new string[] { "Mircea", "Simona", "Florentina", "Vasile", "Ioana", "Mihail", "Diana", "Adrian", "Nicoleta", "Stefan" }},
                {"Nicolescu", new string[] { "Paul", "Mara", "Viorica", "Sergiu", "Carmen", "Alex", "Bianca", "Liviu", "Sofia", "Cristian" }},
                {"Dinu", new string[] { "Claudia", "Daniel", "Adriana", "Ion", "Alexandra", "Elena", "Lucian", "Corina", "Andrei", "Tudor" }},
                {"Tanase", new string[] { "Vlad", "Irina", "Roxana", "Teodor", "Eliza", "Florin", "Alina", "Marius", "Radu", "Cristina" }},
                {"Enescu", new string[] { "Mihai", "Anca", "Silvia", "Bogdan", "Raluca", "Ciprian", "Gabriela", "Sorin", "Ileana", "Teodor" }},
                {"Popa", new string[] { "George", "Irina", "Daniel", "Ana", "Cătălin", "Ramona", "Andrei", "Mihaela", "Victor", "Nicoleta" }},
                {"Vasilescu", new string[] { "Vasile", "Simona", "Elena", "Teodor", "Laura", "Marian", "Carmen", "Mihai", "Anca", "Florin" }},
                {"Matei", new string[] { "Ionel", "Maria", "Valentin", "Cristina", "Bogdan", "Ioana", "Victor", "Nicoleta", "Alexandru", "Gabriela" }},
                {"Sandu", new string[] { "Adrian", "Loredana", "Florin", "Laura", "Cătălin", "Simona", "Ionuț", "Elena", "Andrei", "Oana" }},
                {"Gheorghe", new string[] { "Ion", "Cristina", "Mircea", "Daniela", "Vasile", "Laura", "Adrian", "Diana", "Florin", "Carmen" }},
                {"Barbu", new string[] { "Paul", "Ana", "Mihai", "Elena", "Cristian", "Raluca", "Teodor", "Iulia", "Liviu", "Bianca" }},
                {"Diaconu", new string[] { "Ioan", "Simona", "Marian", "Cristina", "Alexandru", "Irina", "Gabriel", "Anca", "Radu", "Elena" }},
                {"Munteanu", new string[] { "Roxana", "Adrian", "Cătălina", "George", "Laura", "Mihai", "Carmen", "Daniel", "Andreea", "Sorin" }},
                {"Voicu", new string[] { "Vlad", "Monica", "Florin", "Gabriela", "Sergiu", "Eliza", "Ionuț", "Adriana", "Ioan", "Raluca" }},
                {"Trandafir", new string[] { "Radu", "Claudia", "Alexandru", "Simona", "Cătălin", "Maria", "Mihai", "Ana", "Adrian", "Gabriela" }}
            };

        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _appDbContext;


        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        
        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }


        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode = 500)
        {
            var model = new ErrorVm
            {
                Schema = User.GetClaimValue(CustomClaimTypes.AuthScheme),
                Url = $"{HttpContext.Request.Path}{HttpContext.Request.QueryString}",
                Title = "A apărut o eroare",
                Message = Messages.RequestError,
                TraceIdentifier = HttpContext.TraceIdentifier,
                ActivityId = Activity.Current?.Id,
                StatusCode = HttpContext.Response.StatusCode,
            };

            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature != null)
            {
                var exception = exceptionHandlerPathFeature.Error;
                _logger.LogError(exception, exceptionHandlerPathFeature.Path);

                model.Message = $"{Messages.RequestError} (Excepție:{exception.HResult})";
                model.StatusCode = 500;
            }

            if (statusCode != model.StatusCode)
                model.StatusCode = statusCode;

            return View(model);
        }


        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GenerateData()
        {           
            var marciSiModeleKeys = _marciSiModeleRandom.Keys.ToArray();

            var random = new Random();

            for (int i = 0; i<100; i++)
            {
                var iKeyMarca = random.Next(0, marciSiModeleKeys.Length-1);

                var marca = marciSiModeleKeys[iKeyMarca];

                var modele = _marciSiModeleRandom[marca];

                var iKeyModel = random.Next(0, modele.Length - 1);

                var model = modele[iKeyModel];

                var m = new Masina()
                {
                    Id = Guid.NewGuid(),
                    Marca = marca,
                    NumarDeKilometri = random.Next(0, 100) * 100,
                    AnFabricatie = random.Next(2015, 2024),
                    Model = model,
                    SerieDeSasiu = $"{Guid.NewGuid()}".ToUpper().Substring(0,3) + $"-{i:D3}-" + $"{Guid.NewGuid()}".ToUpper().Replace("-","").Substring(0, 9),
                    NumarDeInmatriculare = $"B{random.Next(100,999)}{(char)random.Next('A', 'Z' + 1)}{(char)random.Next('A', 'Z' + 1)}{(char)random.Next('A', 'Z' + 1)}",
                };                

                _appDbContext.Masini.Add(m);
            }

            var numeKeys = _numeSiPrenumeRandom.Keys.ToArray();

            for (int i = 0; i < 300; i++)
            {
                var iKeyNume = random.Next(0, numeKeys.Length);
                var nume = numeKeys[iKeyNume];
                var prenume = _numeSiPrenumeRandom[nume][random.Next(0, _numeSiPrenumeRandom[nume].Length)];

                var a = new Angajat()
                {
                    Id = Guid.NewGuid(),
                    Nume = nume,
                    Prenume = prenume,
                    Telefon = $"07{random.Next(20,25)} {random.Next(100, 999)} {random.Next(100, 999)}",
                    Email = $"{prenume.ToLower()}.{nume.ToLower()}-{i:D3}@gmail.com",
                    Marca = 1001 + i
                };

                var c = $"{random.Next(73, 99) + 7}{random.Next(1, 12):D2}{random.Next(1, 28):D2}{random.Next(1000, 99999)}";

                a.Cnp = prenume.EndsWith('a') ? $"2{c}" : $"1{c}";

                _appDbContext.Angajati.Add(a);
            }

            await _appDbContext.SaveChangesAsync();

            _logger.LogInformation("S-au inserat date aut generate");

            return View();
        }


        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteData()
        {
            await _appDbContext.Database.ExecuteSqlAsync($@"
delete from dbo.deplasari;
delete from dbo.masini;
delete from dbo.angajati;
");

            _logger.LogInformation("S-au șters toate datele");

            return View();
        }
    }
}
