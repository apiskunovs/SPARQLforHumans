using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SparqlForHumans.Lucene;
using SparqlForHumans.Lucene.Extensions;
using SparqlForHumans.Lucene.Queries;
using SparqlForHumans.Lucene.Queries.Fields;
using SparqlForHumans.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SparqlForHumans.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FindPropertiesController : Controller
    {
        private readonly NLog.Logger _logger = Logger.Logger.Init();
        private readonly LuceneConfig _luceneConfig;

        public FindPropertiesController(LuceneConfig luceneConfig)
        {
            this._luceneConfig = luceneConfig;
        }

        public IActionResult Run(string words, string domain, string range, string id, string idNot, int limit = 20)
        {
            _logger.Info("FindPropertiesController Started");
            _logger.Info($"Params words={words}, domain={domain}, range={range}, id={id}, idNot={idNot}, limit={limit}");
            var filteredItems = new FindPropertiesQuery(this._luceneConfig.PropertyIndexPath, words, domain, range, id, idNot, limit).Query(limit);
            _logger.Info($"FindPropertiesController Ended [{filteredItems.ToList<Property>().Count}]");
            return Json(filteredItems);
        }
    }
}
