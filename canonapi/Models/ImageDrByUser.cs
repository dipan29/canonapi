using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace canonapi.Models
{
    public class ImageDrByUser
    {
        public long id { get; set; }
        public string imagename { get; set; }
        public int kaggle_sushrut_drmatched { get; set; }
        public int drlevel_byuser { get; set; }
        public string subdiseaseids { get; set; }
        public int userid { get; set; }
        public DateTime createdon { get; set; }
        public DateTime? modifiedon { get; set; }
        public string regionannotation { get; set; }
        public string markedforreview { get; set; }
    }

    public class ImageDrByUserFinal
    {
        public long id { get; set; }
        public string imagename { get; set; }
        public int kaggle_sushrut_drmatched { get; set; }
        public int drlevel_byuser { get; set; }
        public IEnumerable<int> subdiseaseids { get; set; }
        public int userid { get; set; }
        public DateTime createdon { get; set; }
        public DateTime? modifiedon { get; set; }
        public AnnotationObject regionannotation { get; set; }
        public string markedforreview { get; set; }
    }

    public class UpdateAnnotationMarkedForReview
    {
        public int predictionid { get; set; }
        public ReviewAction action { get; set; }
    }
    
    public class MarkedForReview
    {
        public static string requested = "requested";
        public static string reviewed = "reviewed";
        public static string completed = "completed";
    }

    public enum ReviewAction
    {
        Review = 0,
        Complete = 1,
        Cancel = 2
    }
}
