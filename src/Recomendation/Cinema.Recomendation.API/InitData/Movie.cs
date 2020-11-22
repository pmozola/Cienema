using System;
using System.Collections.Generic;

namespace Cinema.Recomendation.API.InitData
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Result2
    {
        public string Name { get; set; }
    }

    public class Directors
    {
        public List<Result2> Results { get; set; }
    }

    public class Result3
    {
        public string Genre { get; set; }
    }

    public class Genres
    {
        public List<Result3> Results { get; set; }
    }

    public class Result4
    {
        public string Name { get; set; }
    }

    public class Stars
    {
        public List<Result4> Results { get; set; }
    }

    public class Result
    {
        public DateTime CreatedAt { get; set; }
        public Directors Directors { get; set; }
        public Genres Genres { get; set; }
        public string Id { get; set; }
        public string Screentime { get; set; }
        public Stars Stars { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
    }

    public class Movies
    {
        public List<Result> Results { get; set; }
    }

    public class MovieData
    {
        public Movies Movies { get; set; }
    }

    public class RootMovieData
    {
        public MovieData Data { get; set; }
    }
}
