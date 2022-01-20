using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace canonapi.Models
{
    public class DatasetMap
    {
        public int id { get; set; }
        public int userid { get; set; }
        public int datasetid { get; set; }
        public bool isadmin { get; set; }
        public bool isanonymous { get; set; }
    }
}
