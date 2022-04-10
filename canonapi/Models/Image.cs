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
        public string location { get; set; }
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
        public Fruits drlevel_kaggle { get; set; }
        public Fruits drlevel_sushrut { get; set; }
        public Fruits drlevel_byuser { get; set; }
        public IEnumerable<int> subdiseaseids { get; set; }
        public string thumbnail { get; set; }
        public string image { get; set; }
    }

    public class ImageOutWithAnnotation
    {
        public long id { get; set; }
        public string imagename { get; set; }
        public Fruits drlevel_kaggle { get; set; }
        public Fruits drlevel_sushrut { get; set; }
        public Fruits drlevel_byuser { get; set; }
        public IEnumerable<int> subdiseaseids { get; set; }
        public string thumbnail { get; set; }
        public string image { get; set; }
        public AnnotationObject regionannotation { get; set; }
        public int datasetid { get; set; }
        public bool superadmin { get; set; }
        public bool is_admin { get; set; }
        public bool is_anonymous { get; set; }
        public string markedforreview { get; set; }
        public List<UsersPrediction> users_prediction { get; set; }
    }

    public class UsersPrediction
    {
        public string username { get; set; }
        public int userid { get; set; }
        public long predictionid { get; set; }
        public Fruits dr_level { get; set; }
        public string markedforreview { get; set; }
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
        public long totalApplesFromPredicted { get; set; }
        public long totalOrangesFromPredicted { get; set; }
        public long totalMangosFromPredicted { get; set; }
        public long totalGrapesFromPredicted { get; set; }
        public long totalBananasFromPredicted { get; set; }
        public long totalOthersFromPredicted { get; set; }
        public long totalUngradableImages { get; set; }
    }

    public class CountMaster
    {
        public decimal Kaggle_Ungraded { get; set; }
        public decimal Sushrut_Ungraded { get; set; }
        public decimal Kaggle_Apples { get; set; }
        public decimal Sushrut_Apples { get; set; }
        public decimal Kaggle_Oranges { get; set; }
        public decimal Sushrut_Oranges { get; set; }
        public decimal Kaggle_Mangos { get; set; }
        public decimal Sushrut_Mangos { get; set; }
        public decimal Kaggle_Grapes { get; set; }
        public decimal Sushrut_Grapes { get; set; }
        public decimal Kaggle_Bananas { get; set; }
        public decimal Sushrut_Bananas { get; set; }
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

    public enum Fruits
    {
        All = -1,//-2,
        //Ungraded = -1,
        Apples = 0,
        Oranges = 1,
        Mangos = 2,
        Grapes = 3,
        Bananas = 4,
        Others = 5,
        Ungradable = 6,
        Ungraded = 7
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
