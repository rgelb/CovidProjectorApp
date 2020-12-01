using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidProjectorAPI.Models
{
    public class CountyAverageModel : AverageModel
    {
        public string Name { get; set; }
    }

    public class StateAverageModel : AverageModel
    {
        public string Name { get; set; }
    }

    public class CountryAverageModel : AverageModel
    {
        public string Name { get; set; }
    }

    public class AverageModel
    {
        public int AverageCases14 { get; set; }
        public int AverageCases7 { get; set; }
        public int AverageCases3 { get; set; }
        public int AverageDeaths14 { get; set; }
        public int AverageDeaths7 { get; set; }
        public int AverageDeaths3 { get; set; }
    }
}
