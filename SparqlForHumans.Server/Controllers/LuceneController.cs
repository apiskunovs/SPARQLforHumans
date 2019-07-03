﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SparqlForHumans.Lucene.Extensions;
using SparqlForHumans.Lucene.Queries;

namespace SparqlForHumans.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/MultiEntityQuery")]
    public class MultiEntityQueryController : Controller
    {
        /*
         * TODO: Al hacer el autoComplete, me muestra las propiedades y las entidades.
         * Debería mostrarme solo las propiedes supongo. No?
         */
        public IActionResult Run(string term)
        {
            var filteredItems = MultiDocumentQueries.QueryEntitiesByLabel(term);
            filteredItems = filteredItems.Select(x => x.AddProperties());

            return Json(filteredItems);
        }
    }
}