using Microsoft.AspNetCore.Mvc;
using SparqlForHumans.Lucene;
using SparqlForHumans.Lucene.Extensions;
using SparqlForHumans.Lucene.Queries;
using SparqlForHumans.Lucene.Queries.Fields;

namespace SparqlForHumans.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MultiEntityQueryController : Controller
    {
        private readonly NLog.Logger _logger = Logger.Logger.Init();

        public IActionResult Run(string term)
        {
            _logger.Info("MultiEntityQuery Start");
            _logger.Info($"Incoming term: {term}");
            var filteredItems = new MultiLabelEntityQuery(LuceneDirectoryDefaults.EntityIndexPath, term).Query();
            _logger.Info("MultiEntityQuery Query");
            return Json(filteredItems);
        }
    }
}