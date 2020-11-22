using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema.Recomendation.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecomendationController : ControllerBase
    {
        private readonly IDriver neoDriver;

        public RecomendationController(IDriver neoDriver)
        {
            this.neoDriver = neoDriver;
        }
        [HttpGet]
        public IActionResult Get([FromQuery] string title)
        {
            var recomednations = GetRecomendation(title).ToList();


            return Ok(recomednations);
        }

        private List<RecomendationResult> GetRecomendation(string title)
        {
            var result = new List<RecomendationResult>();
            using var session = neoDriver.Session();

            var queryResult = session.Run(@$"
                MATCH (Movie {{title: ""{title}""}})<-[:ACTED_IN]-(actors), (actors)-[:ACTED_IN]-(moviePlayed)
                
                with collect({{ title: moviePlayed.title}}) as rows
                
                MATCH(Movie {{ title: ""{title}""}})< -[:DIRECTED] - (directors), (directors) -[:DIRECTED] - (movieDirected)
                   with rows + collect({{ title: movieDirected.title}}) as allRows
                
                UNWIND allRows as row
                with row.title as title
                return title as Title, count(*) as Strenght
                order by Strenght DESC
                limit 5");
      
            foreach (var element in queryResult)
            {
                result.Add(new RecomendationResult
                {
                    Name = element.Values["Title"] as string,
                    Strenght = int.Parse(element.Values["Strenght"].ToString())
                });
            }
            return result;
        }
    }

    public class RecomendationResult
    {
        public string Name { get; set; }
        public int Strenght { get; set; }
    }

}
