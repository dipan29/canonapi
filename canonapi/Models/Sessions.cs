using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace canonapi.Models
{
    public class Sessions
    {
        public int id { get; set; }
        public int userid { get; set; }
        public DateTime logintime { get; set; }
        public DateTime lastactivitytime { get; set; }
        public string sessionkey { get; set; }
    }
}
