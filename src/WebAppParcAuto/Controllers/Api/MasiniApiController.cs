using Common.Extensions;
using Common.Models;
using Database.DataModels;
using Database.ScalarMappedFunctions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WebAppParcAuto.Controllers.Api
{
    public class MasiniApiController : Controller
    {
        private readonly AppDbContext _appDbContext;


        public MasiniApiController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        [HttpGet]
        public ActionResult Select2Masini(string searchTerm, int? pageSize, int? pageNumber)
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

            var listaMasiniQueryable = _appDbContext.Masini.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                listaMasiniQueryable = listaMasiniQueryable.Where(a =>
                    CharIndex.Udf(searchTerm, a.Marca) > 0 ||
                    CharIndex.Udf(searchTerm, a.Model) > 0 ||
                    CharIndex.Udf(searchTerm, a.NumarDeInmatriculare) > 0);

                select2Pagination.IsFiltered = true;
            }
            else
                select2Pagination.IsFiltered = false;

            select2Pagination.TotalRecords = listaMasiniQueryable.Count();
            select2Pagination.PageSize = pageSize ?? 50;

            select2Pagination.PageNumber = pageNumber ?? 0;
            select2Pagination.PageLast = select2Pagination.TotalRecords <= 0 ?  0 : (select2Pagination.TotalRecords + select2Pagination.PageSize - 1) / select2Pagination.PageSize;

            if (select2Pagination.PageNumber < 0 || select2Pagination.PageNumber > select2Pagination.PageLast)
                select2Pagination.PageNumber = select2Pagination.PageLast;

            if (select2Pagination.PageNumber == 0)
                select2Pagination.PageNumber = 1;

            foreach(var masina in listaMasiniQueryable.Skip((select2Pagination.PageNumber - 1) * select2Pagination.PageSize).Take(select2Pagination.PageSize).ToList())
            {
                var item = new Select2ItemData
                {
                    Id = $"{masina.Id}",
                    Text = $"{masina.NumarDeInmatriculare} {masina.NumarDeKilometri} KM",
                    Tag = []
                };

                item.Tag.Add("Marca",masina.Marca);
                item.Tag.Add("Model", masina.Model);
                item.Tag.Add("AnFabricatie", masina.AnFabricatie);
                item.Tag.Add("NumarDeInmatriculare", masina.NumarDeInmatriculare);
                item.Tag.Add("NumarDeKilometri", masina.NumarDeKilometri);

                select2Data.Add(item);
            }

            return Content(JsonConvert.SerializeObject(new {
                Results = select2Data,
                Pagination = select2Pagination,
            }, Formatting.None), "application/json");
        }
    }
}
