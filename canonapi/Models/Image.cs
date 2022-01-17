using System.Collections.Generic;

namespace canonapi.Models
{
    public class Image
    {
        public long id { get; set; }
        public string imagename { get; set; }
        public int drlevel_kaggle { get; set; }
        public int drlevel_sushrut { get; set; }
        public string imageurl { get; set; }
        public int datasetid { get; set; }
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
        public DRStatus drlevel_byuser { get; set; }
        public IEnumerable<int> subdiseaseids { get; set; }
        public string thumbnail { get; set; }
        public string image { get; set; }
    }

    public class ImageOutWithAnnotation
    {
        public long id { get; set; }
        public string imagename { get; set; }
        public DRStatus drlevel_kaggle { get; set; }
        public DRStatus drlevel_sushrut { get; set; }
        public DRStatus drlevel_byuser { get; set; }
        public IEnumerable<int> subdiseaseids { get; set; }
        public string thumbnail { get; set; }
        public string image { get; set; }
        public AnnotationObject regionannotation { get; set; }
    }

    public class ImageOutIds
    {
        public long id { get; set; }
    }

    public class UserWisePredictionCounts
    {
        public long totalImages { get; set; }
        public long totalImagesPredicted { get; set; }
        public long totalUngradedImages { get; set; }
        public long totalDR0FromPredicted { get; set; }
        public long totalDR1FromPredicted { get; set; }
        public long totalDR2FromPredicted { get; set; }
        public long totalDR3FromPredicted { get; set; }
        public long totalDR4FromPredicted { get; set; }
        public long totalOthersFromPredicted { get; set; }
        public long totalUngradableImages { get; set; }
    }

    public class CountMaster
    {
        public decimal Kaggle_Ungraded { get; set; }
        public decimal Sushrut_Ungraded { get; set; }
        public decimal Kaggle_DR0 { get; set; }
        public decimal Sushrut_DR0 { get; set; }
        public decimal Kaggle_DR1 { get; set; }
        public decimal Sushrut_DR1 { get; set; }
        public decimal Kaggle_DR2 { get; set; }
        public decimal Sushrut_DR2 { get; set; }
        public decimal Kaggle_DR3 { get; set; }
        public decimal Sushrut_DR3 { get; set; }
        public decimal Kaggle_DR4 { get; set; }
        public decimal Sushrut_DR4 { get; set; }
        public decimal Kaggle_Others { get; set; }
        public decimal Sushrut_Others { get; set; }
        public decimal Kaggle_Ungradable { get; set; }
        public decimal Sushrut_Ungradable { get; set; }
    }

    public class SerializedCountResult
    {
        public KaggleAndSushrutMatchedImages IsMatchedBucket { get; set; }
        public IEnumerable<CountMaster> counts { get; set; }
    }

    public enum DRStatus
    {
        All = -2,
        Ungraded = -1,
        DR0 = 0,
        DR1 = 1,
        DR2 = 2,
        DR3 = 3,
        DR4 = 4,
        Others = 5,
        Ungradable = 6
    }

    public enum KaggleAndSushrutMatchedImages
    {
        No = 0,
        Yes = 1,
        All = 2
    }

    public enum DataSource
    {
        Both = 0,
        Sushrut = 1,
        Kaggle = 2
    }
}
