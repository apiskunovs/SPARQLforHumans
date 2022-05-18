using Microsoft.AspNetCore.Mvc;
using SparqlForHumans.Lucene;
using SparqlForHumans.Lucene.Extensions;
using SparqlForHumans.Lucene.Queries;
using SparqlForHumans.Lucene.Queries.Fields;

namespace SparqlForHumans.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FindInstancesController : Controller
    {
        private readonly NLog.Logger _logger = Logger.Logger.Init();
        private readonly LuceneConfig _luceneConfig;

        public FindInstancesController(LuceneConfig luceneConfig)
        {
            this._luceneConfig = luceneConfig;
        }

        public IActionResult Run(string words, string instanceOf, string inProp, string outProp, string id, string idNot, string isType, int limit = 20)
        {
            _logger.Info("FindInstancesController Started");
            _logger.Info($"Params words={words}, instanceOf={instanceOf}, inProp={inProp}, outProp={outProp}, limit={limit},, id={id}, idNot={idNot}, isType={isType}");
            var filteredItems = new FindInstancesQuery(this._luceneConfig.EntityIndexPath, words, instanceOf, inProp, outProp, id, idNot, isType, limit).Query();
            _logger.Info("FindInstancesController Ended");
            return Json(filteredItems);
        }
    }
}
