using Neo4j.Driver;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cinema.Recomendation.API.InitData
{
    public static class InitDataManager
    {
        public static void Init(IDriver neoDriver)
        {
            using var session = neoDriver.Session();

            if (IsAnyDataInDatabase(session))
            {
                return;
            }

            var jsonString = File.ReadAllText("movies.json");
            var rootMovieData = JsonConvert.DeserializeObject<RootMovieData>(jsonString);

            foreach (var movie in rootMovieData.Data.Movies.Results.Where(x => !string.IsNullOrWhiteSpace(x.Year)))
            {
                var sb = new StringBuilder();
                var movieShortTitle = OnlyLetters(movie.Title);
                sb.AppendLine( $"CREATE({movieShortTitle}:Movie {{ title:\"{movie.Title}\", released:{OnlyDigits(movie.Year)}}}) ");

               
                foreach (var actor in movie.Stars.Results)
                {
                    var actorName = OnlyLetters(actor.Name);
                    sb.AppendLine($"CREATE ({actorName}:Person {{name:'{actor.Name}'}}) ");
                    sb.AppendLine($"CREATE ({actorName})-[:ACTED_IN ]->({movieShortTitle}) ");
                }
                foreach (var director in movie.Directors.Results)
                {
                    var directorName = OnlyLetters(director.Name);

                    if (!DirectorPlayingInMovie(movie.Stars.Results, director.Name))
                    {
                        sb.AppendLine($"CREATE ({directorName}:Person {{name:'{director.Name}'}})");
                    }
                    
                    sb.AppendLine($"CREATE ({directorName})-[:DIRECTED]->({movieShortTitle})");
                }

                session.WriteTransaction(tx => tx.Run(sb.ToString()));
            }

        }
        private static bool IsAnyDataInDatabase(ISession neoSession) 
        {
            var numberOfNodes = neoSession.WriteTransaction(tx =>
            {
                var result = tx.Run("MATCH (n) RETURN count(n)");
                return result.Single()[0].As<int>();
            });
            return numberOfNodes > 0;
        }

        private static bool DirectorPlayingInMovie(List<Result4> actors, string name)
        {
            return actors.Any(x => x.Name == name);
        }

        private static string OnlyLetters(string text)
        {
            return Regex.Replace(text, "[^a-zA-Z]", "");
        }

        private static string OnlyDigits(string text)
        {
            return Regex.Replace(text, "[^0-9]", "");
        }
    }
}
