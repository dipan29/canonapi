namespace canonapi.Models
{
    public class Image
    {
        public long id { get; set; }
        public string imagename { get; set; }
        public int drlevel_kaggle { get; set; }
        public int drlevel_sushrut { get; set; }
        public string imageurl { get; set; }
    }

    public class Imagebackup
    {
        public long id { get; set; }
        public string imagename { get; set; }
        public int drlevel_kaggle { get; set; }
        public int drlevel_sushrut { get; set; }
        public string imageurl { get; set; }
    }

    public class ImageOut
    {
        public long id { get; set; }
        public DRStatus drlevel_kaggle { get; set; }
        public DRStatus drlevel_sushrut { get; set; }
        public string thumbnail { get; set; }
        public string image { get; set; }
    }

    public enum DRStatus
    {
        DR0 = 0,
        DR1 = 1,
        DR2 = 2,
        DR3 = 3,
        DR4 = 4
    }
}
