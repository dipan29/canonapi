using System.Collections.Generic;

namespace canonapi.Models
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string userpassword { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public bool admin { get; set; }
    }

    public class DatasetsForUser
    {
        public User user { get; set; }
        public List<MappedDatasets> lstMappedDatasets { get; set; }
    }

    public class MappedDatasets
    {
        public int datasetid { get; set; }
        public string datasetname { get; set; }
        public string description { get; set; }
        public string attribute { get; set; }
        public string referenceid { get; set; }
        public string adminid { get; set; }
        public bool isadmin { get; set; }
        public bool isanonymous { get; set; }
    }
}
