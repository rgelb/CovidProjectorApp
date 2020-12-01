using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidProjectorAPI.Models
{


    public class CountiesCovidDataModel
    {
        public string help { get; set; }
        public bool success { get; set; }
        public Result result { get; set; }
    }

    public class Result
    {
        public Record[] records { get; set; }
        public Field[] fields { get; set; }
        public string sql { get; set; }
    }

    public class Record
    {
        public string totalcountconfirmed { get; set; }
        public string newcountdeaths { get; set; }
        public string totalcountdeaths { get; set; }
        public string _full_text { get; set; }
        public string county { get; set; }
        public string newcountconfirmed { get; set; }
        public DateTime date { get; set; }
        public int _id { get; set; }
    }

    public class Field
    {
        public string type { get; set; }
        public string id { get; set; }
    }




    //public class StateCovidDataModel
    //{
    //    public string help { get; set; }
    //    public bool success { get; set; }
    //    public Result result { get; set; }
    //}

    //public class Result
    //{
    //    public bool include_total { get; set; }
    //    public string resource_id { get; set; }
    //    public Field[] fields { get; set; }
    //    public string records_format { get; set; }
    //    public Record[] records { get; set; }
    //    public int limit { get; set; }
    //    public _Links _links { get; set; }
    //    public int total { get; set; }
    //}

    //public class _Links
    //{
    //    public string start { get; set; }
    //    public string next { get; set; }
    //}

    //public class Field
    //{
    //    public string type { get; set; }
    //    public string id { get; set; }
    //    public Info info { get; set; }
    //}

    //public class Info
    //{
    //    public string notes { get; set; }
    //    public string type_override { get; set; }
    //    public string label { get; set; }
    //}

    //public class Record
    //{
    //    public float totalcountconfirmed { get; set; }
    //    public int newcountdeaths { get; set; }
    //    public float totalcountdeaths { get; set; }
    //    public string county { get; set; }
    //    public int newcountconfirmed { get; set; }
    //    public DateTime date { get; set; }
    //    public int _id { get; set; }
    //}


}
