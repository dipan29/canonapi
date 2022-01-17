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
}
