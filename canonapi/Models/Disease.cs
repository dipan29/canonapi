using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace canonapi.Models
{
    public class Disease
    {
        public int diseaseid { get; set; }
        public string diseasename { get; set; }
        public int subdiseaseid { get; set; }
        public string subdieseasename { get; set; }
        public string colorname { get; set; }
        public string colorcode { get; set; }
    }
}
