using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidProjectorAPI.Models
{
    public class CountyDayModel
    {
        public string Name { get; set; }
        public int Cases { get; set; }
        public int Deaths { get; set; }
        public DateTime Date { get; set; }
    }
}
