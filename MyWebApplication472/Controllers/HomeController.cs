using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Queries;

namespace MyWebApplication472.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var store = new DocumentStore { Urls = new[] { "http://localhost:8082" }, Database = "MyTest" };
            store.Initialize();
            IndexCreation.CreateIndexes(typeof(Employees_En_Gb).Assembly, store);

            var searchTerms = "Sales";

            IList<Employee> employees;
            int totalItems;
            using (var session = store.OpenSession())
            {
                var query = session.Advanced.DocumentQuery<Employee, Employees_En_Gb>().UsingDefaultOperator(QueryOperator.And).Statistics(out var stats);
                query = searchTerms.ToLower().Split(new[] { " ", "-", "/", "\\" }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (q, value) => q.WhereStartsWith(x => x.Title, value));

                //var query = session.Query<Product, Products_It_It>()
                //    .Search(x => x.Title,
                //        searchTerms.ToLower().Split(new[] {" ", "-", "/", "\\"},
                //            StringSplitOptions.RemoveEmptyEntries).Select(s => $"{s}*"), 1, SearchOptions.Guess, SearchOperator.And).Statistics(out var stats);


                employees = query.Take(50).ToList();
                totalItems = stats.TotalResults;
            }

            var sb = new StringBuilder();
            sb.Append($"Found {totalItems} items...");
            foreach (var product in employees)
                sb.AppendLine(product.Title);

            ViewBag.Message(sb.ToString());

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}