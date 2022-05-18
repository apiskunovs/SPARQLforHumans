using Microsoft.AspNetCore.Mvc;
using SparqlForHumans.Lucene;
using SparqlForHumans.Lucene.Extensions;
using SparqlForHumans.Lucene.Queries;
using SparqlForHumans.Lucene.Queries.Fields;
using SparqlForHumans.Models;
using System.Collections.Generic;
using System.Linq;

namespace SparqlForHumans.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class GetPrevPropertiesController : Controller
    {
        private readonly NLog.Logger _logger = Logger.Logger.Init();
        private readonly LuceneConfig _luceneConfig;

        public GetPrevPropertiesController(LuceneConfig luceneConfig)
        {
            this._luceneConfig = luceneConfig;
        }

        public IActionResult Run(string id, int limit = 20)
        {
            _logger.Info("GetPrevPropertiesController Started");
            _logger.Info($"Params id={id}, limit={limit}");
            var filteredItems = new FindPropertyAroundPropertyQuery(this._luceneConfig.EntityIndexPath, this._luceneConfig.PropertyIndexPath, id, FindPropertyAroundPropertyQuery.FindMode.PreviousProperties, limit).Query();
            _logger.Info($"GetPrevPropertiesController resulted in {filteredItems.Count()} hits");

            return Json(filteredItems);
        }
    }
}
