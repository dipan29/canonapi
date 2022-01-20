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

    public class UsersForDataset
    {
        public Datasets dataset { get; set; }
        public List<MappedUsers> lstMappedUsers { get; set; }
    }

    public class MappedUsers
    {
        public int userid { get; set; }
        public string username { get; set; }
        //public string userpassword { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public bool adminuser { get; set; }
        public bool isadmin { get; set; }
        public bool isanonymous { get; set; }
    }
}
