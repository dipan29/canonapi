using canonapi.Authentication;
using canonapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class BucketCalculator
{
    public int CalculateSpecificBucketByCondition(ApplicationDbContext _dbContext, Fruits dr, KaggleAndSushrutMatchedImages matchingscope, int userid, string datasetids = null)
    {
        try
        {
            List<DatasetMap> lstDatasetMap = GetDatasetMapping(_dbContext, userid, datasetids);
            int[] arrDatasetIds = lstDatasetMap.Select(i => i.datasetid).ToArray();

            int totalDRx = default(int);

            if (dr == Fruits.Ungraded)
            {
                switch (matchingscope)
                {
                    case KaggleAndSushrutMatchedImages.Yes:
                        {
                            IEnumerable<string> sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                                                && arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();

                            IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                                                && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()).Select(i => i.imagename).ToList();

                            //Getting the final list
                            IEnumerable<string> UnGradedImages = sourceDRxImages.Where(f => !userDRxImages.Contains(f));
                            //Get counts
                            totalDRx = UnGradedImages.Count();
                        }
                        break;
                    case KaggleAndSushrutMatchedImages.No:
                        {
                            IEnumerable<string> sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                                                && arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();

                            IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                                        && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()).Select(i => i.imagename).ToList();

                            //Getting the final list
                            IEnumerable<string> UnGradedImages = sourceDRxImages.Where(f => !userDRxImages.Contains(f));
                            //Get counts
                            totalDRx = UnGradedImages.Count();
                        }
                        break;
                    default:
                        {
                            IEnumerable<string> sourceDRxImages = _dbContext.images.Where(i => arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();

                            IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid).Select(i => i.imagename).ToList();

                            //Getting the final list
                            IEnumerable<string> UnGradedImages = sourceDRxImages.Where(f => !userDRxImages.Contains(f));
                            //Get counts
                            totalDRx = UnGradedImages.Count();
                        }
                        break;
                }
            }
            else
            {
                List<string> AllowedImages = _dbContext.images.Where(i => arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                int[] arrDatasetsNotAnonymous = _dbContext.datasetmap.Where(d => d.isanonymous == false && d.userid == userid).Select(d => d.id).ToArray();
                if (matchingscope == KaggleAndSushrutMatchedImages.Yes)
                {
                    IEnumerable<string> sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                                //&& i.drlevel_sushrut == dr.GetHashCode() && arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                && i.drlevel_sushrut == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                    ///////////////////////////////
                    List<string> sourceDRxImagesCopy = new List<string>();
                    sourceDRxImagesCopy.AddRange(sourceDRxImages);

                    IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                    && u.drlevel_byuser == dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
                    ///////////////////////////////
                    List<string> userDRxImagesCopy = new List<string>();
                    userDRxImagesCopy.AddRange(userDRxImages);

                    IEnumerable<string> userNotDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                    && u.drlevel_byuser != dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
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
                    IEnumerable<string> sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                                && (i.drlevel_sushrut == dr.GetHashCode() || i.drlevel_kaggle == dr.GetHashCode())
                                //&& arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                    ///////////////////////////////
                    List<string> sourceDRxImagesCopy = new List<string>();
                    sourceDRxImagesCopy.AddRange(sourceDRxImages);

                    IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                    && u.drlevel_byuser == dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
                    ///////////////////////////////
                    List<string> userDRxImagesCopy = new List<string>();
                    userDRxImagesCopy.AddRange(userDRxImages);

                    IEnumerable<string> userNotDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                    && u.drlevel_byuser != dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
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
                    IEnumerable<string> sourceDRxImages = _dbContext.images.Where(i => i.drlevel_sushrut == dr.GetHashCode()
                    //|| i.drlevel_kaggle == dr.GetHashCode() && arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                    || i.drlevel_kaggle == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                    ///////////////////////////////
                    List<string> sourceDRxImagesCopy = new List<string>();
                    sourceDRxImagesCopy.AddRange(sourceDRxImages);

                    IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.drlevel_byuser == dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
                    ///////////////////////////////
                    List<string> userDRxImagesCopy = new List<string>();
                    userDRxImagesCopy.AddRange(userDRxImages);

                    IEnumerable<string> userNotDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.drlevel_byuser != dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
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
            }
            return totalDRx;
        }
        catch (Exception ex)
        {
            return default(int);
        }
    }

    public int CalculateSpecificBucketByPredictionSourceCondition(ApplicationDbContext _dbContext, Fruits dr, KaggleAndSushrutMatchedImages matchingscope, int userid, DataSource source = DataSource.Both, string datasetids = null)
    {
        int totalDRx = default(int);
        try
        {
            List<DatasetMap> lstDatasetMap = GetDatasetMapping(_dbContext, userid, datasetids);
            int[] arrDatasetIds = lstDatasetMap.Select(i => i.datasetid).ToArray();

            if (dr == Fruits.Ungraded)
            {
                switch (matchingscope)
                {
                    case KaggleAndSushrutMatchedImages.Yes:
                        {
                            IEnumerable<string> sourceDRxImages = new List<string>();
                            sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                            && arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();

                            IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                                        && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()).Select(i => i.imagename).ToList();

                            //Getting the final list
                            IEnumerable<string> UnGradedImages = sourceDRxImages.Where(f => !userDRxImages.Contains(f));
                            //Get counts
                            totalDRx = UnGradedImages.Count();
                        }
                        break;
                    case KaggleAndSushrutMatchedImages.No:
                        {
                            IEnumerable<string> sourceDRxImages = new List<string>();
                            sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                            && arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();

                            IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                                        && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()).Select(i => i.imagename).ToList();

                            //Getting the final list
                            IEnumerable<string> UnGradedImages = sourceDRxImages.Where(f => !userDRxImages.Contains(f));
                            //Get counts
                            totalDRx = UnGradedImages.Count();
                        }
                        break;
                    default:
                        {
                            IEnumerable<string> sourceDRxImages = new List<string>();
                            sourceDRxImages = _dbContext.images.Where(i => arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();

                            IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid).Select(i => i.imagename).ToList();

                            //Getting the final list
                            IEnumerable<string> UnGradedImages = sourceDRxImages.Where(f => !userDRxImages.Contains(f));
                            //Get counts
                            totalDRx = UnGradedImages.Count();
                        }
                        break;
                }
            }
            else
            {
                List<string> AllowedImages = _dbContext.images.Where(i => arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                int[] arrDatasetsNotAnonymous = _dbContext.datasetmap.Where(d => d.isanonymous == false && d.userid == userid).Select(d => d.id).ToArray();
                if (matchingscope == KaggleAndSushrutMatchedImages.Yes)
                {
                    IEnumerable<string> sourceDRxImages = new List<string>();
                    switch (source)
                    {
                        case DataSource.Kaggle:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                                && i.drlevel_kaggle == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                        case DataSource.Sushrut:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                                && i.drlevel_sushrut == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                        default:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                                && i.drlevel_sushrut == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                    }
                    ///////////////////////////////
                    List<string> sourceDRxImagesCopy = new List<string>();
                    sourceDRxImagesCopy.AddRange(sourceDRxImages);

                    IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                    && u.drlevel_byuser == dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
                    ///////////////////////////////
                    List<string> userDRxImagesCopy = new List<string>();
                    userDRxImagesCopy.AddRange(userDRxImages);

                    IEnumerable<string> userNotDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                    && u.drlevel_byuser != dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
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
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                                && i.drlevel_kaggle == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                        case DataSource.Sushrut:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                                && i.drlevel_sushrut == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                        default:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                                && (i.drlevel_sushrut == dr.GetHashCode() || i.drlevel_kaggle == dr.GetHashCode())
                                && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                    }
                    ///////////////////////////////
                    List<string> sourceDRxImagesCopy = new List<string>();
                    sourceDRxImagesCopy.AddRange(sourceDRxImages);

                    IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                    && u.drlevel_byuser == dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
                    ///////////////////////////////
                    List<string> userDRxImagesCopy = new List<string>();
                    userDRxImagesCopy.AddRange(userDRxImages);

                    IEnumerable<string> userNotDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                    && u.drlevel_byuser != dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
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
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle == dr.GetHashCode()
                                && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                        case DataSource.Sushrut:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_sushrut == dr.GetHashCode()
                                && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                        default:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_sushrut == dr.GetHashCode()
                                || i.drlevel_kaggle == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                    }
                    ///////////////////////////////
                    List<string> sourceDRxImagesCopy = new List<string>();
                    sourceDRxImagesCopy.AddRange(sourceDRxImages);

                    IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.drlevel_byuser == dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
                    ///////////////////////////////
                    List<string> userDRxImagesCopy = new List<string>();
                    userDRxImagesCopy.AddRange(userDRxImages);

                    IEnumerable<string> userNotDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.drlevel_byuser != dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
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
            }
            return totalDRx;
        }
        catch (Exception ex)
        {
            return default(int);
        }
    }

    public IEnumerable<long> GetAllIdsForSpecificBucketByPredictionSourceCondition(ApplicationDbContext _dbContext, Fruits dr, KaggleAndSushrutMatchedImages matchingscope, int userid, DataSource source = DataSource.Both, string datasetids = null)
    {
        IEnumerable<long> DRxIds = new List<long>();
        try
        {
            List<DatasetMap> lstDatasetMap = GetDatasetMapping(_dbContext, userid, datasetids);
            int[] arrDatasetIds = lstDatasetMap.Select(i => i.datasetid).ToArray();

            if (dr == Fruits.Ungraded)
            {
                switch (matchingscope)
                {
                    case KaggleAndSushrutMatchedImages.Yes:
                        {
                            IEnumerable<string> sourceDRxImages = new List<string>();
                            sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                                        && arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();

                            IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                                        && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()).Select(i => i.imagename).ToList();

                            //Getting the final list
                            IEnumerable<string> UnGradedImages = sourceDRxImages.Where(f => !userDRxImages.Contains(f));
                            //Get Ids
                            DRxIds = _dbContext.images.Where(i => UnGradedImages.Contains(i.imagename)).Select(i => i.id).ToList();
                        }
                        break;
                    case KaggleAndSushrutMatchedImages.No:
                        {
                            IEnumerable<string> sourceDRxImages = new List<string>();
                            sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                                        && arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();

                            IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                                        && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()).Select(i => i.imagename).ToList();

                            //Getting the final list
                            IEnumerable<string> UnGradedImages = sourceDRxImages.Where(f => !userDRxImages.Contains(f));
                            //Get Ids
                            DRxIds = _dbContext.images.Where(i => UnGradedImages.Contains(i.imagename)).Select(i => i.id).ToList();
                        }
                        break;
                    default:
                        {
                            IEnumerable<string> sourceDRxImages = new List<string>();
                            sourceDRxImages = _dbContext.images.Where(i => arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();

                            IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid).Select(i => i.imagename).ToList();

                            //Getting the final list
                            IEnumerable<string> UnGradedImages = sourceDRxImages.Where(f => !userDRxImages.Contains(f));
                            //Get Ids
                            DRxIds = _dbContext.images.Where(i => UnGradedImages.Contains(i.imagename)).Select(i => i.id).ToList();
                        }
                        break;
                }
            }
            else
            {
                List<string> AllowedImages = _dbContext.images.Where(i => arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                int[] arrDatasetsNotAnonymous = _dbContext.datasetmap.Where(d => d.isanonymous == false && d.userid == userid).Select(d => d.id).ToArray();
                if (matchingscope == KaggleAndSushrutMatchedImages.Yes)
                {
                    IEnumerable<string> sourceDRxImages = new List<string>();
                    switch (source)
                    {
                        case DataSource.Kaggle:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                                && i.drlevel_kaggle == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                        case DataSource.Sushrut:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                                && i.drlevel_sushrut == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                        default:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut
                                && i.drlevel_sushrut == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                    }
                    ///////////////////////////////
                    List<string> sourceDRxImagesCopy = new List<string>();
                    sourceDRxImagesCopy.AddRange(sourceDRxImages);

                    IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                    && u.drlevel_byuser == dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
                    ///////////////////////////////
                    List<string> userDRxImagesCopy = new List<string>();
                    userDRxImagesCopy.AddRange(userDRxImages);

                    IEnumerable<string> userNotDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                    && u.drlevel_byuser != dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
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
                                DRxIds = _dbContext.images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                                break;
                            }
                        case DataSource.Sushrut:
                            {
                                DRxIds = _dbContext.images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                                break;
                            }
                        default:
                            {
                                DRxIds = _dbContext.images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
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
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                                && i.drlevel_kaggle == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                        case DataSource.Sushrut:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                                && i.drlevel_sushrut == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                        default:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut
                                && (i.drlevel_sushrut == dr.GetHashCode() || i.drlevel_kaggle == dr.GetHashCode())
                                && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                    }
                    ///////////////////////////////
                    List<string> sourceDRxImagesCopy = new List<string>();
                    sourceDRxImagesCopy.AddRange(sourceDRxImages);

                    IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                    && u.drlevel_byuser == dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
                    ///////////////////////////////
                    List<string> userDRxImagesCopy = new List<string>();
                    userDRxImagesCopy.AddRange(userDRxImages);

                    IEnumerable<string> userNotDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.kaggle_sushrut_drmatched == matchingscope.GetHashCode()
                    && u.drlevel_byuser != dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
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
                                DRxIds = _dbContext.images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                                break;
                            }
                        case DataSource.Sushrut:
                            {
                                DRxIds = _dbContext.images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                                break;
                            }
                        default:
                            {
                                DRxIds = _dbContext.images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
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
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_kaggle == dr.GetHashCode()
                                && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                        case DataSource.Sushrut:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_sushrut == dr.GetHashCode()
                                && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                        default:
                            {
                                sourceDRxImages = _dbContext.images.Where(i => i.drlevel_sushrut == dr.GetHashCode()
                                || i.drlevel_kaggle == dr.GetHashCode() && arrDatasetsNotAnonymous.Contains(i.datasetid)).Select(i => i.imagename).ToList();
                                break;
                            }
                    }
                    ///////////////////////////////
                    List<string> sourceDRxImagesCopy = new List<string>();
                    sourceDRxImagesCopy.AddRange(sourceDRxImages);

                    IEnumerable<string> userDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.drlevel_byuser == dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
                    ///////////////////////////////
                    List<string> userDRxImagesCopy = new List<string>();
                    userDRxImagesCopy.AddRange(userDRxImages);

                    IEnumerable<string> userNotDRxImages = _dbContext.imagedrbyusers.Where(u => u.userid == userid
                    && u.drlevel_byuser != dr.GetHashCode() && AllowedImages.Contains(u.imagename)).Select(i => i.imagename).ToList();
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
                                DRxIds = _dbContext.images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                                break;
                            }
                        case DataSource.Sushrut:
                            {
                                DRxIds = _dbContext.images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                                break;
                            }
                        default:
                            {
                                DRxIds = _dbContext.images.Where(i => newDRxMainMinusUncommon.Contains(i.imagename)).Select(i => i.id).ToList();
                                break;
                            }
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

    public List<DatasetMap> GetDatasetMapping(ApplicationDbContext _dbContext, int userid, string datasetids = null)
    {
        try
        {
            List<DatasetMap> objDatasetMap = null;
            if (String.IsNullOrEmpty(datasetids))
            {
                objDatasetMap = _dbContext.datasetmap.Where(i => i.userid == userid).ToList();
            }
            else
            {
                string[] arrDatasetIds = datasetids.Trim().Split(',');
                objDatasetMap = _dbContext.datasetmap.Where(i => i.userid == userid && arrDatasetIds.Contains(i.datasetid.ToString())).ToList();
            }
            return objDatasetMap;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
