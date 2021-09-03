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

    public class UserWisePredictionCounts
    {
        public long totalImages { get; set; }
        public long totalImagesPredicted { get; set; }
        public long totalDR0FromPredicted { get; set; }
        public long totalDR1FromPredicted { get; set; }
        public long totalDR2FromPredicted { get; set; }
        public long totalDR3FromPredicted { get; set; }
        public long totalDR4FromPredicted { get; set; }
    }

    public enum DRStatus
    {
        DR0 = 0,
        DR1 = 1,
        DR2 = 2,
        DR3 = 3,
        DR4 = 4,
        All = 5
    }

    public enum KaggleAndSushrutMatchedImages
    {
        No = 0,
        Yes = 1,
        All = 2
    }
}
