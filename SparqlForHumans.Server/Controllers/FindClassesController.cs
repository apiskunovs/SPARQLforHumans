using Microsoft.AspNetCore.Mvc;
using SparqlForHumans.Lucene;
using SparqlForHumans.Lucene.Extensions;
using SparqlForHumans.Lucene.Queries;
using SparqlForHumans.Lucene.Queries.Fields;

namespace SparqlForHumans.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FindClassesController : Controller
    {
        private readonly NLog.Logger _logger = Logger.Logger.Init();
        private readonly LuceneConfig _luceneConfig;

        public FindClassesController(LuceneConfig luceneConfig)
        {
            this._luceneConfig = luceneConfig;
        }

        public IActionResult Run(string words, string instanceOf, string inProp, string outProp, string id, string idNot, int limit = 20)
        {
            _logger.Info("FindClassesController Started");
            _logger.Info($"Params words={words}, instanceOf={instanceOf}, inProp={inProp}, outProp={outProp}, limit={limit},, id={id}, idNot={idNot}");
            var filteredItems = new FindInstancesQuery(this._luceneConfig.EntityIndexPath, words, instanceOf, inProp, outProp, id, idNot, true.ToString(), limit).Query();
            _logger.Info("FindClassesController Ended");
            return Json(filteredItems);
        }
    }
}
