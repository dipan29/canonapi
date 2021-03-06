using canonapi.Authentication;
using canonapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class BucketCalculator
{
    public int CalculateSpecificBucketByCondition(ApplicationDbContext _dbContext, DRStatus dr, KaggleAndSushrutMatchedImages matchingscope, int userid)
    {
        try
        {
            int totalDRx = default(int);
            if (matchingscope == KaggleAndSushrutMatchedImages.Yes)
            {
                IEnumerable<string> sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                            && i.drlevel_sushrut == dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> sourceDRxImagesCopy = new List<string>();
                sourceDRxImagesCopy.AddRange(sourceDRxImages);

                IEnumerable<string> userDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                && u.drlevel_byuser == dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userDRxImagesCopy = new List<string>();
                userDRxImagesCopy.AddRange(userDRxImages);

                IEnumerable<string> userNotDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                && u.drlevel_byuser != dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userNotDRxImagesCopy = new List<string>();
                userNotDRxImagesCopy.AddRange(userNotDRxImages);

                //Getting uncommon DRx
                userDRxImagesCopy.RemoveAll(x => sourceDRxImagesCopy.Exists(y => y == x));
                //Getting common plus remain DRx from source bucket
                IEnumerable<string> newDRx = sourceDRxImages.Union(userDRxImages);
                //Getting the final list
                IEnumerable<string> newDRxMainMinusUncommon = newDRx.ToList().Where(f => !userNotDRxImagesCopy.Contains(f));
                //Get counts
                totalDRx = newDRxMainMinusUncommon.Count();
            }
            else if (matchingscope == KaggleAndSushrutMatchedImages.No)
            {
                IEnumerable<string> sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                            && (i.drlevel_sushrut == dr.GetHashCode() || i.drlevel_kaggle == dr.GetHashCode())).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> sourceDRxImagesCopy = new List<string>();
                sourceDRxImagesCopy.AddRange(sourceDRxImages);

                IEnumerable<string> userDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                && u.drlevel_byuser == dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userDRxImagesCopy = new List<string>();
                userDRxImagesCopy.AddRange(userDRxImages);

                IEnumerable<string> userNotDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                && u.drlevel_byuser != dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userNotDRxImagesCopy = new List<string>();
                userNotDRxImagesCopy.AddRange(userNotDRxImages);

                //Getting uncommon DRx
                userDRxImagesCopy.RemoveAll(x => sourceDRxImagesCopy.Exists(y => y == x));
                //Getting common plus remain DRx from source bucket
                IEnumerable<string> newDRx = sourceDRxImages.Union(userDRxImages);
                //Getting the final list
                IEnumerable<string> newDRxMainMinusUncommon = newDRx.ToList().Where(f => !userNotDRxImagesCopy.Contains(f));
                //Get counts
                totalDRx = newDRxMainMinusUncommon.Count();
            }
            else
            {
                IEnumerable<string> sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_sushrut == dr.GetHashCode()
                || i.drlevel_kaggle == dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> sourceDRxImagesCopy = new List<string>();
                sourceDRxImagesCopy.AddRange(sourceDRxImages);

                IEnumerable<string> userDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.drlevel_byuser == dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userDRxImagesCopy = new List<string>();
                userDRxImagesCopy.AddRange(userDRxImages);

                IEnumerable<string> userNotDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.drlevel_byuser != dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userNotDRxImagesCopy = new List<string>();
                userNotDRxImagesCopy.AddRange(userNotDRxImages);

                //Getting uncommon DRx
                userDRxImagesCopy.RemoveAll(x => sourceDRxImagesCopy.Exists(y => y == x));
                //Getting common plus remain DRx from source bucket
                IEnumerable<string> newDRx = sourceDRxImages.Union(userDRxImages);
                //Getting the final list
                IEnumerable<string> newDRxMainMinusUncommon = newDRx.ToList().Where(f => !userNotDRxImagesCopy.Contains(f));
                //Get counts
                totalDRx = newDRxMainMinusUncommon.Count();
            }
            return totalDRx;
        }
        catch (Exception ex)
        {
            return default(int);
        }
    }

    public int CalculateSpecificBucketByPredictionSourceCondition(ApplicationDbContext _dbContext, DRStatus dr, KaggleAndSushrutMatchedImages matchingscope, int userid, DataSource source = DataSource.Both)
    {
        int totalDRx = default(int);
        try
        {
            if (matchingscope == KaggleAndSushrutMatchedImages.Yes)
            {
                IEnumerable<string> sourceDRxImages = new List<string>();
                switch (source)
                {
                    case DataSource.Kaggle:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                            && i.drlevel_kaggle == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                    case DataSource.Sushrut:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                            && i.drlevel_sushrut == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                    default:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                            && i.drlevel_sushrut == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                }
                ///////////////////////////////
                List<string> sourceDRxImagesCopy = new List<string>();
                sourceDRxImagesCopy.AddRange(sourceDRxImages);

                IEnumerable<string> userDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                && u.drlevel_byuser == dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userDRxImagesCopy = new List<string>();
                userDRxImagesCopy.AddRange(userDRxImages);

                IEnumerable<string> userNotDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                && u.drlevel_byuser != dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userNotDRxImagesCopy = new List<string>();
                userNotDRxImagesCopy.AddRange(userNotDRxImages);

                //Getting uncommon DRx
                userDRxImagesCopy.RemoveAll(x => sourceDRxImagesCopy.Exists(y => y == x));
                //Getting common plus remain DRx from source bucket
                IEnumerable<string> newDRx = sourceDRxImages.Union(userDRxImages);
                //Getting the final list
                IEnumerable<string> newDRxMainMinusUncommon = newDRx.ToList().Where(f => !userNotDRxImagesCopy.Contains(f));
                //Get counts
                totalDRx = newDRxMainMinusUncommon.Count();
            }
            else if (matchingscope == KaggleAndSushrutMatchedImages.No)
            {
                IEnumerable<string> sourceDRxImages = new List<string>();
                switch (source)
                {
                    case DataSource.Kaggle:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                            && i.drlevel_kaggle == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                    case DataSource.Sushrut:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                            && i.drlevel_sushrut == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                    default:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                            && (i.drlevel_sushrut == dr.GetHashCode() || i.drlevel_kaggle == dr.GetHashCode())).Select(i => i.imagename).ToList();
                            break;
                        }
                }
                ///////////////////////////////
                List<string> sourceDRxImagesCopy = new List<string>();
                sourceDRxImagesCopy.AddRange(sourceDRxImages);

                IEnumerable<string> userDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                && u.drlevel_byuser == dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userDRxImagesCopy = new List<string>();
                userDRxImagesCopy.AddRange(userDRxImages);

                IEnumerable<string> userNotDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                && u.drlevel_byuser != dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userNotDRxImagesCopy = new List<string>();
                userNotDRxImagesCopy.AddRange(userNotDRxImages);

                //Getting uncommon DRx
                userDRxImagesCopy.RemoveAll(x => sourceDRxImagesCopy.Exists(y => y == x));
                //Getting common plus remain DRx from source bucket
                IEnumerable<string> newDRx = sourceDRxImages.Union(userDRxImages);
                //Getting the final list
                IEnumerable<string> newDRxMainMinusUncommon = newDRx.ToList().Where(f => !userNotDRxImagesCopy.Contains(f));
                //Get counts
                totalDRx = newDRxMainMinusUncommon.Count();
            }
            else
            {
                IEnumerable<string> sourceDRxImages = new List<string>();
                switch (source)
                {
                    case DataSource.Kaggle:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                    case DataSource.Sushrut:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_sushrut == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                    default:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_sushrut == dr.GetHashCode()
                            || i.drlevel_kaggle == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                }
                ///////////////////////////////
                List<string> sourceDRxImagesCopy = new List<string>();
                sourceDRxImagesCopy.AddRange(sourceDRxImages);

                IEnumerable<string> userDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.drlevel_byuser == dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userDRxImagesCopy = new List<string>();
                userDRxImagesCopy.AddRange(userDRxImages);

                IEnumerable<string> userNotDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.drlevel_byuser != dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userNotDRxImagesCopy = new List<string>();
                userNotDRxImagesCopy.AddRange(userNotDRxImages);

                //Getting uncommon DRx
                userDRxImagesCopy.RemoveAll(x => sourceDRxImagesCopy.Exists(y => y == x));
                //Getting common plus remain DRx from source bucket
                IEnumerable<string> newDRx = sourceDRxImages.Union(userDRxImages);
                //Getting the final list
                IEnumerable<string> newDRxMainMinusUncommon = newDRx.ToList().Where(f => !userNotDRxImagesCopy.Contains(f));
                //Get counts
                totalDRx = newDRxMainMinusUncommon.Count();
            }
            return totalDRx;
        }
        catch (Exception ex)
        {
            return default(int);
        }
    }

    public IEnumerable<long> GetAllIdsForSpecificBucketByPredictionSourceCondition(ApplicationDbContext _dbContext, DRStatus dr, KaggleAndSushrutMatchedImages matchingscope, int userid, DataSource source = DataSource.Both)
    {
        IEnumerable<long> DRxIds = new List<long>();
        try
        {
            if (matchingscope == KaggleAndSushrutMatchedImages.Yes)
            {
                IEnumerable<string> sourceDRxImages = new List<string>();
                switch (source)
                {
                    case DataSource.Kaggle:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                            && i.drlevel_kaggle == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                    case DataSource.Sushrut:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                            && i.drlevel_sushrut == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                    default:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                            && i.drlevel_sushrut == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                }
                ///////////////////////////////
                List<string> sourceDRxImagesCopy = new List<string>();
                sourceDRxImagesCopy.AddRange(sourceDRxImages);

                IEnumerable<string> userDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                && u.drlevel_byuser == dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userDRxImagesCopy = new List<string>();
                userDRxImagesCopy.AddRange(userDRxImages);

                IEnumerable<string> userNotDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                && u.drlevel_byuser != dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userNotDRxImagesCopy = new List<string>();
                userNotDRxImagesCopy.AddRange(userNotDRxImages);

                //Getting uncommon DRx
                userDRxImagesCopy.RemoveAll(x => sourceDRxImagesCopy.Exists(y => y == x));
                //Getting common plus remain DRx from source bucket
                IEnumerable<string> newDRx = sourceDRxImages.Union(userDRxImages);
                //Getting the final list
                IEnumerable<string> newDRxMainMinusUncommon = newDRx.ToList().Where(f => !userNotDRxImagesCopy.Contains(f));
                //Get Ids by condition
                switch (source)
                {
                    case DataSource.Kaggle:
                        {
                            DRxIds = _dbContext.Images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                            break;
                        }
                    case DataSource.Sushrut:
                        {
                            DRxIds = _dbContext.Images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                            break;
                        }
                    default:
                        {
                            DRxIds = _dbContext.Images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                            break;
                        }
                }
            }
            else if (matchingscope == KaggleAndSushrutMatchedImages.No)
            {
                IEnumerable<string> sourceDRxImages = new List<string>();
                switch (source)
                {
                    case DataSource.Kaggle:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                            && i.drlevel_kaggle == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                    case DataSource.Sushrut:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                            && i.drlevel_sushrut == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                    default:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                            && (i.drlevel_sushrut == dr.GetHashCode() || i.drlevel_kaggle == dr.GetHashCode())).Select(i => i.imagename).ToList();
                            break;
                        }
                }
                ///////////////////////////////
                List<string> sourceDRxImagesCopy = new List<string>();
                sourceDRxImagesCopy.AddRange(sourceDRxImages);

                IEnumerable<string> userDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                && u.drlevel_byuser == dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userDRxImagesCopy = new List<string>();
                userDRxImagesCopy.AddRange(userDRxImages);

                IEnumerable<string> userNotDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                && u.drlevel_byuser != dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userNotDRxImagesCopy = new List<string>();
                userNotDRxImagesCopy.AddRange(userNotDRxImages);

                //Getting uncommon DRx
                userDRxImagesCopy.RemoveAll(x => sourceDRxImagesCopy.Exists(y => y == x));
                //Getting common plus remain DRx from source bucket
                IEnumerable<string> newDRx = sourceDRxImages.Union(userDRxImages);
                //Getting the final list
                IEnumerable<string> newDRxMainMinusUncommon = newDRx.ToList().Where(f => !userNotDRxImagesCopy.Contains(f));
                //Get Ids by condition
                switch (source)
                {
                    case DataSource.Kaggle:
                        {
                            DRxIds = _dbContext.Images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                            break;
                        }
                    case DataSource.Sushrut:
                        {
                            DRxIds = _dbContext.Images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                            break;
                        }
                    default:
                        {
                            DRxIds = _dbContext.Images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                            break;
                        }
                }
            }
            else
            {
                IEnumerable<string> sourceDRxImages = new List<string>();
                switch (source)
                {
                    case DataSource.Kaggle:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_kaggle == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                    case DataSource.Sushrut:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_sushrut == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                    default:
                        {
                            sourceDRxImages = _dbContext.Images.Where(i => i.drlevel_sushrut == dr.GetHashCode()
                            || i.drlevel_kaggle == dr.GetHashCode()).Select(i => i.imagename).ToList();
                            break;
                        }
                }
                ///////////////////////////////
                List<string> sourceDRxImagesCopy = new List<string>();
                sourceDRxImagesCopy.AddRange(sourceDRxImages);

                IEnumerable<string> userDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.drlevel_byuser == dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userDRxImagesCopy = new List<string>();
                userDRxImagesCopy.AddRange(userDRxImages);

                IEnumerable<string> userNotDRxImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userid
                && u.drlevel_byuser != dr.GetHashCode()).Select(i => i.imagename).ToList();
                ///////////////////////////////
                List<string> userNotDRxImagesCopy = new List<string>();
                userNotDRxImagesCopy.AddRange(userNotDRxImages);

                //Getting uncommon DRx
                userDRxImagesCopy.RemoveAll(x => sourceDRxImagesCopy.Exists(y => y == x));
                //Getting common plus remain DRx from source bucket
                IEnumerable<string> newDRx = sourceDRxImages.Union(userDRxImages);
                //Getting the final list
                IEnumerable<string> newDRxMainMinusUncommon = newDRx.ToList().Where(f => !userNotDRxImagesCopy.Contains(f));
                //Get Ids by condition
                switch (source)
                {
                    case DataSource.Kaggle:
                        {
                            DRxIds = _dbContext.Images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                            break;
                        }
                    case DataSource.Sushrut:
                        {
                            DRxIds = _dbContext.Images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                            break;
                        }
                    default:
                        {
                            DRxIds = _dbContext.Images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                            break;
                        }
                }
            }
            return DRxIds;
        }
        catch (Exception ex)
        {
            return new List<long>();
        }
    }
}
