using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace canonapi.Models
{
    public class Datasets
    {
        public int id { get; set; }
        public string datasetname { get; set; }
        public string description { get; set; }
        public string attribute { get; set; }
        public string referenceid { get; set; }
        public string adminid { get; set; }
    }
}
