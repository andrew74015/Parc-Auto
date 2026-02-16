using Common.Extensions;
using Common.Models;
using Database.DataModels;
using Database.ScalarMappedFunctions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WebAppParcAuto.Controllers.Api
{
    public class AngajatiApiController : Controller
    {
        private readonly AppDbContext _appDbContext;


        public AngajatiApiController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        [HttpGet]
        public ActionResult Select2Angajati(string searchTerm, int? pageSize, int? pageNumber)
        {
            searchTerm ??= string.Empty;

            var select2Data = new List<Select2ItemData>();
            var select2Pagination = new Select2Pagination();

            if (!User.GetIsAuthenticated())
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    Results = select2Data,
                    Pagination = select2Pagination,
                }, Formatting.None), "application/json");
            }

            var listaAngajatiQueryable = _appDbContext.Angajati.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                listaAngajatiQueryable = listaAngajatiQueryable.Where(a =>
                    CharIndex.Udf(searchTerm, a.Nume) > 0 ||
                    CharIndex.Udf(searchTerm, a.Prenume) > 0 ||
                    CharIndex.Udf(searchTerm, a.Email) > 0);

                select2Pagination.IsFiltered = true;
            }
            else
                select2Pagination.IsFiltered = false;

            select2Pagination.TotalRecords = listaAngajatiQueryable.Count();
            select2Pagination.PageSize = pageSize ?? 50;

            select2Pagination.PageNumber = pageNumber ?? 0;
            select2Pagination.PageLast = select2Pagination.TotalRecords <= 0 ?  0 : (select2Pagination.TotalRecords + select2Pagination.PageSize - 1) / select2Pagination.PageSize;

            if (select2Pagination.PageNumber < 0 || select2Pagination.PageNumber > select2Pagination.PageLast)
                select2Pagination.PageNumber = select2Pagination.PageLast;

            if (select2Pagination.PageNumber == 0)
                select2Pagination.PageNumber = 1;

            foreach(var angajat in listaAngajatiQueryable.Skip((select2Pagination.PageNumber - 1) * select2Pagination.PageSize).Take(select2Pagination.PageSize).ToList())
            {
                var item = new Select2ItemData
                {
                    Id = $"{angajat.Id}",
                    Text = $"{angajat.Nume} {angajat.Prenume}",
                    Tag = []
                };

                item.Tag.Add("Email",angajat.Email);
                item.Tag.Add("Telefon", angajat.Telefon);
                item.Tag.Add("Marca", angajat.Marca);

                select2Data.Add(item);
            }

            return Content(JsonConvert.SerializeObject(new {
                Results = select2Data,
                Pagination = select2Pagination,
            }, Formatting.None), "application/json");
        }
    }
}
