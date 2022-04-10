﻿using canonapi.Authentication;
using canonapi.configuration;
using canonapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace canonapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ImagesController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IAnnotationService _annotationService;

        public ImagesController(ApplicationDbContext context, IConfiguration configuration, IAnnotationService annotationService)
        {
            _configuration = configuration;
            _dbContext = context;
            _annotationService = annotationService;
        }

        [HttpGet]
        [ActionName("GetImageCount")]
        public IActionResult GetImageCount(string datasetids = null)
        {
            UserWisePredictionCounts counts = new UserWisePredictionCounts();
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);

                List<DatasetMap> lstDatasetMap = new BucketCalculator().GetDatasetMapping(_dbContext, userObj.id, datasetids);
                int[] arrDatasetIds = lstDatasetMap.Select(i => i.datasetid).ToArray();

                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    counts.totalImages = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut && arrDatasetIds.Contains(i.datasetid)).Count();
                    //counts.totalImagesPredicted = _dbContext.imagedrbyusers.Where(i => i.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode() && i.userid == userObj.id).Count();
                    //Ungraded Count
                    {
                        counts.totalUngradedImages = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Ungraded, KaggleAndSushrutMatchedImages.Yes, userObj.id, datasetids);
                    }
                    counts.totalImagesPredicted = counts.totalImages - counts.totalUngradedImages;
                    //DR0 Count
                    {
                        counts.totalApplesFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Apples, KaggleAndSushrutMatchedImages.Yes, userObj.id, datasetids);
                    }
                    //DR1 Count
                    {
                        counts.totalOrangesFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Oranges, KaggleAndSushrutMatchedImages.Yes, userObj.id, datasetids);
                    }
                    //DR2 Count
                    {
                        counts.totalMangosFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Mangos, KaggleAndSushrutMatchedImages.Yes, userObj.id, datasetids);
                    }
                    //DR3 Count
                    {
                        counts.totalGrapesFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Grapes, KaggleAndSushrutMatchedImages.Yes, userObj.id, datasetids);
                    }
                    //DR4 Count
                    {
                        counts.totalBananasFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Bananas, KaggleAndSushrutMatchedImages.Yes, userObj.id, datasetids);
                    }
                    //Others Count
                    {
                        counts.totalOthersFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Others, KaggleAndSushrutMatchedImages.Yes, userObj.id, datasetids);
                    }
                    //Ungradable Count
                    {
                        counts.totalUngradableImages = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Ungradable, KaggleAndSushrutMatchedImages.Yes, userObj.id, datasetids);
                    }
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    counts.totalImages = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut).Count();
                    //counts.totalImagesPredicted = _dbContext.imagedrbyusers.Where(i => i.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()).Count();
                    //Ungraded Count
                    {
                        counts.totalUngradedImages = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Ungraded, KaggleAndSushrutMatchedImages.No, userObj.id, datasetids);
                    }
                    counts.totalImagesPredicted = counts.totalImages - counts.totalUngradedImages;
                    //DR0 Count
                    {
                        counts.totalApplesFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Apples, KaggleAndSushrutMatchedImages.No, userObj.id, datasetids);
                    }
                    //DR1 Count
                    {
                        counts.totalOrangesFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Oranges, KaggleAndSushrutMatchedImages.No, userObj.id, datasetids);
                    }
                    //DR2 Count
                    {
                        counts.totalMangosFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Mangos, KaggleAndSushrutMatchedImages.No, userObj.id, datasetids);
                    }
                    //DR3 Count
                    {
                        counts.totalGrapesFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Grapes, KaggleAndSushrutMatchedImages.No, userObj.id, datasetids);
                    }
                    //DR4 Count
                    {
                        counts.totalBananasFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Bananas, KaggleAndSushrutMatchedImages.No, userObj.id, datasetids);
                    }
                    //Others Count
                    {
                        counts.totalOthersFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Others, KaggleAndSushrutMatchedImages.No, userObj.id, datasetids);
                    }
                    //Ungradable Count
                    {
                        counts.totalUngradableImages = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            Fruits.Ungradable, KaggleAndSushrutMatchedImages.No, userObj.id, datasetids);
                    }
                }
                else
                {

                }

                return Ok(new
                {
                    success = 1,
                    data = counts
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [HttpGet]
        [ActionName("GetImageCountEx")]
        public IActionResult GetImageCountEx(string datasetids = null)
        {
            UserWisePredictionCounts counts = new UserWisePredictionCounts();
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);

                List<DatasetMap> lstDatasetMap = new BucketCalculator().GetDatasetMapping(_dbContext, userObj.id, datasetids);
                int[] arrDatasetIds = lstDatasetMap.Select(i => i.datasetid).ToArray();
                List<string> AllowedImages = _dbContext.images.Where(i => arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();

                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    counts.totalImagesPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    ////counts.totalUngradedImages = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    ////&& u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    ////&& u.drlevel_byuser == DRStatus.Ungraded.GetHashCode()).Count();

                    counts.totalApplesFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == Fruits.Apples.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalOrangesFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == Fruits.Oranges.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalMangosFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == Fruits.Mangos.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalGrapesFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == Fruits.Grapes.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalBananasFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == Fruits.Bananas.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalOthersFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == Fruits.Others.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalUngradableImages = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == Fruits.Ungradable.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalImages = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut && arrDatasetIds.Contains(i.datasetid)).Count();

                    counts.totalUngradedImages = counts.totalImages - counts.totalImagesPredicted;
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    counts.totalImagesPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    ////counts.totalUngradedImages = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    ////&& u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    ////&& u.drlevel_byuser == DRStatus.Ungraded.GetHashCode()).Count();

                    counts.totalApplesFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == Fruits.Apples.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalOrangesFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == Fruits.Oranges.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalMangosFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == Fruits.Mangos.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalGrapesFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == Fruits.Grapes.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalBananasFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == Fruits.Bananas.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalOthersFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == Fruits.Others.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalUngradableImages = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == Fruits.Ungradable.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalImages = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut && arrDatasetIds.Contains(i.datasetid)).Count();

                    counts.totalUngradedImages = counts.totalImages - counts.totalImagesPredicted;
                }
                else
                {
                    counts.totalImagesPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    ////counts.totalUngradedImages = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    ////&& u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    ////&& u.drlevel_byuser == DRStatus.Ungraded.GetHashCode()).Count();

                    counts.totalApplesFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == Fruits.Apples.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalOrangesFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == Fruits.Oranges.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalMangosFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == Fruits.Mangos.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalGrapesFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == Fruits.Grapes.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalBananasFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == Fruits.Bananas.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalOthersFromPredicted = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == Fruits.Others.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalUngradableImages = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == Fruits.Ungradable.GetHashCode() && AllowedImages.Contains(u.imagename)).Count();

                    counts.totalImages = _dbContext.images.Where(i => arrDatasetIds.Contains(i.datasetid)).Count();

                    counts.totalUngradedImages = counts.totalImages - counts.totalImagesPredicted;
                }

                return Ok(new
                {
                    success = 1,
                    data = counts
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [HttpGet]
        [ActionName("GetImageCountByBucket")]
        public IActionResult GetImageCountByBucket(string datasetids = null)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);

                List<CountMaster> counts = new List<CountMaster>();
                int xKaggleUngraded, xKaggleDR0, xKaggleDR1, xKaggleDR2, xKaggleDR3, xKaggleDR4, xKaggleOthers, xKaggleUngradable, xSushrutUngraded, xSushrutDR0, xSushrutDR1, xSushrutDR2, xSushrutDR3, xSushrutDR4, xSushrutOthers, xSushrutUngradable;
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    xKaggleUngraded = xSushrutUngraded = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Ungraded, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR0 = xSushrutDR0 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Apples, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR1 = xSushrutDR1 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Oranges, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR2 = xSushrutDR2 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Mangos, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR3 = xSushrutDR3 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Grapes, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR4 = xSushrutDR4 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Bananas, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleOthers = xSushrutOthers = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Others, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleUngradable = xSushrutUngradable = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Ungradable, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut, datasetids);

                    counts.Add(new CountMaster()
                    {
                        Kaggle_Ungraded = xKaggleUngraded,
                        Kaggle_Apples = xKaggleDR0,
                        Kaggle_Oranges = xKaggleDR1,
                        Kaggle_Mangos = xKaggleDR2,
                        Kaggle_Grapes = xKaggleDR3,
                        Kaggle_Bananas = xKaggleDR4,
                        Kaggle_Others = xKaggleOthers,
                        Kaggle_Ungradable = xKaggleUngradable,
                        Sushrut_Ungraded = xSushrutUngraded,
                        Sushrut_Apples = xSushrutDR0,
                        Sushrut_Oranges = xSushrutDR1,
                        Sushrut_Mangos = xSushrutDR2,
                        Sushrut_Grapes = xSushrutDR3,
                        Sushrut_Bananas = xSushrutDR4,
                        Sushrut_Others = xSushrutOthers,
                        Sushrut_Ungradable = xSushrutUngradable
                    });
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    xKaggleUngraded = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Ungraded, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutUngraded = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Ungraded, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR0 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Apples, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutDR0 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Apples, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR1 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Oranges, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutDR1 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Oranges, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR2 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Mangos, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutDR2 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Mangos, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR3 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Grapes, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutDR3 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Grapes, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR4 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Bananas, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutDR4 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Bananas, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleOthers = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Others, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutOthers = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Others, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleUngradable = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Ungradable, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutUngradable = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Ungradable, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut, datasetids);

                    counts.Add(new CountMaster()
                    {
                        Kaggle_Ungraded = xKaggleUngraded,
                        Kaggle_Apples = xKaggleDR0,
                        Kaggle_Oranges = xKaggleDR1,
                        Kaggle_Mangos = xKaggleDR2,
                        Kaggle_Grapes = xKaggleDR3,
                        Kaggle_Bananas = xKaggleDR4,
                        Kaggle_Others = xKaggleOthers,
                        Kaggle_Ungradable = xKaggleUngradable,
                        Sushrut_Ungraded = xSushrutUngraded,
                        Sushrut_Apples = xSushrutDR0,
                        Sushrut_Oranges = xSushrutDR1,
                        Sushrut_Mangos = xSushrutDR2,
                        Sushrut_Grapes = xSushrutDR3,
                        Sushrut_Bananas = xSushrutDR4,
                        Sushrut_Others = xSushrutOthers,
                        Sushrut_Ungradable = xSushrutUngradable
                    });
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.All)
                {
                    xKaggleUngraded = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Ungraded, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutUngraded = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Ungraded, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR0 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Apples, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutDR0 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Apples, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR1 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Oranges, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutDR1 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Oranges, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR2 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Mangos, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutDR2 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Mangos, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR3 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Grapes, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutDR3 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Grapes, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleDR4 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Bananas, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutDR4 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Bananas, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleOthers = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Others, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutOthers = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Others, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut, datasetids);

                    xKaggleUngradable = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Ungradable, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle, datasetids);
                    xSushrutUngradable = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, Fruits.Ungradable, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut, datasetids);

                    counts.Add(new CountMaster()
                    {
                        Kaggle_Ungraded = xKaggleUngraded,
                        Kaggle_Apples = xKaggleDR0,
                        Kaggle_Oranges = xKaggleDR1,
                        Kaggle_Mangos = xKaggleDR2,
                        Kaggle_Grapes = xKaggleDR3,
                        Kaggle_Bananas = xKaggleDR4,
                        Kaggle_Others = xKaggleOthers,
                        Kaggle_Ungradable = xKaggleUngradable,
                        Sushrut_Ungraded = xSushrutUngraded,
                        Sushrut_Apples = xSushrutDR0,
                        Sushrut_Oranges = xSushrutDR1,
                        Sushrut_Mangos = xSushrutDR2,
                        Sushrut_Grapes = xSushrutDR3,
                        Sushrut_Bananas = xSushrutDR4,
                        Sushrut_Others = xSushrutOthers,
                        Sushrut_Ungradable = xSushrutUngradable
                    });
                }
                SerializedCountResult result = new SerializedCountResult();
                result.IsMatchedBucket = (KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]);
                result.counts = counts;
                return Ok(new
                {
                    success = 1,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [HttpGet]
        [ActionName("GetImageCountByBucketEx")]
        public IActionResult GetImageCountByBucketEx(string datasetids = null)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);

                string strQueryDataset = !String.IsNullOrEmpty(datasetids) ? (" AND i.datasetid IN (SELECT DISTINCT datasetid from datasetmap where userid = " + userObj.id + " AND datasetid in (" + datasetids + "))")
                                                                            : (" AND i.datasetid IN (SELECT DISTINCT datasetid from datasetmap where userid = " + userObj.id + ")");

                IEnumerable<CountMaster> counts = new List<CountMaster>();
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    counts = _dbContext.ExecuteQuery<CountMaster>(string.Format(QueryHelper.qryMatchedResultCount, userObj.id, strQueryDataset));
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    counts = _dbContext.ExecuteQuery<CountMaster>(string.Format(QueryHelper.qryUnmatchedResultCount, userObj.id, strQueryDataset));
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.All)
                {
                    counts = _dbContext.ExecuteQuery<CountMaster>(string.Format(QueryHelper.qryAllResultCount, userObj.id, strQueryDataset));
                }
                SerializedCountResult result = new SerializedCountResult();
                result.IsMatchedBucket = (KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]);
                result.counts = counts;
                return Ok(new
                {
                    success = 1,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [HttpGet]
        [ActionName("GetPaginated")]
        public IActionResult GetPaginated([FromQuery] int page, [FromQuery] int limit, [FromQuery] Fruits dr = Fruits.All, string datasetids = null)
        {
            List<ImageOut> lstImages = new List<ImageOut>();
            try
            {
                if (page == 0)
                    page = 1;

                if (limit == 0)
                    limit = int.MaxValue;

                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);

                List<DatasetMap> lstDatasetMap = new BucketCalculator().GetDatasetMapping(_dbContext, userObj.id, datasetids);
                int[] arrDatasetIds = lstDatasetMap.Select(i => i.datasetid).ToArray();
                List<string> AllowedImages = _dbContext.images.Where(i => arrDatasetIds.Contains(i.datasetid)).Select(i => i.imagename).ToList();

                var skip = (page - 1) * limit;
                IEnumerable<Image> savedSearches = null;
                IEnumerable<ImageDrByUser> userGradedImages = null;
                IEnumerable<ImageDrByUser> userGradedImagesConditional = null;
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    if (dr == Fruits.All || dr == Fruits.Ungraded)
                    {
                        userGradedImagesConditional = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                        && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode() && AllowedImages.Contains(u.imagename));

                        if (userGradedImagesConditional != null && userGradedImagesConditional.Count() > default(int))
                        {
                            var totRec = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut && arrDatasetIds.Contains(i.datasetid)).ToList();
                            var ugradedImg = userGradedImagesConditional.ToList();
                            savedSearches = (from x in totRec where !ugradedImg.Any(y => y.imagename == x.imagename) select x).ToList().Skip(skip).Take(limit);
                        }
                        else
                        {
                            savedSearches = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut && arrDatasetIds.Contains(i.datasetid)).Skip(skip).Take(limit);
                        }
                    }
                    else
                    {
                        userGradedImages = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                        && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                        && u.drlevel_byuser == dr.GetHashCode() && AllowedImages.Contains(u.imagename));
                    }
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    if (dr == Fruits.All || dr == Fruits.Ungraded)
                    {
                        userGradedImagesConditional = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                        && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode() && AllowedImages.Contains(u.imagename));

                        if (userGradedImagesConditional != null && userGradedImagesConditional.Count() > default(int))
                        {
                            var totRec = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut && arrDatasetIds.Contains(i.datasetid)).ToList();
                            var ugradedImg = userGradedImagesConditional.ToList();
                            savedSearches = (from x in totRec where !ugradedImg.Any(y => y.imagename != x.imagename) select x).ToList().Skip(skip).Take(limit);
                        }
                        else
                        {
                            savedSearches = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut && arrDatasetIds.Contains(i.datasetid)).Skip(skip).Take(limit);
                        }
                    }
                    else
                    {
                        userGradedImages = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                        && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                        && u.drlevel_byuser == dr.GetHashCode() && AllowedImages.Contains(u.imagename));
                    }
                }
                else
                {
                    if (dr == Fruits.All || dr == Fruits.Ungraded)
                    {
                        userGradedImagesConditional = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                        && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode() && AllowedImages.Contains(u.imagename));

                        if (userGradedImagesConditional != null && userGradedImagesConditional.Count() > default(int))
                        {
                            var totRec = _dbContext.images.Where(i => arrDatasetIds.Contains(i.datasetid)).ToList();
                            var ugradedImg = userGradedImagesConditional.ToList();
                            savedSearches = (from x in totRec where !ugradedImg.Any(y => y.imagename == x.imagename) select x).ToList().Skip(skip).Take(limit);
                        }
                        else
                        {
                            savedSearches = _dbContext.images.Where(i => arrDatasetIds.Contains(i.datasetid)).Skip(skip).Take(limit);
                        }
                    }
                    else
                    {
                        userGradedImages = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id
                        && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                        && u.drlevel_byuser == dr.GetHashCode() && AllowedImages.Contains(u.imagename));
                    }
                }

                List<Image> lst = new List<Image>();
                if (savedSearches != null && savedSearches.Count() > default(int))
                {
                    lst = savedSearches.ToList();
                }
                else
                {
                    if (userGradedImagesConditional == null && userGradedImages != null && userGradedImages.Count() > default(int))
                    {
                        userGradedImages.ToList().ForEach(ui =>
                        {
                            var img = new Image();
                            var qryImg = _dbContext.images.Where(i => i.imagename == ui.imagename && arrDatasetIds.Contains(i.datasetid)).FirstOrDefault();
                            img.imagename = qryImg.imagename;
                            img.id = qryImg.id;
                            img.imageurl = qryImg.imageurl;
                            img.drlevel_sushrut = qryImg.drlevel_sushrut;
                            img.drlevel_kaggle = qryImg.drlevel_kaggle;
                            lst.Add(img);
                        });
                    }
                }

                lst.ForEach(i =>
                {
                    var imgOutObj = new ImageOut();
                    imgOutObj.id = i.id;
                    //imgOutObj.thumbnail = new ImageHandler(_configuration).GetFileFromLocal(i.imageurl, true);
                    imgOutObj.drlevel_kaggle = (Fruits)i.drlevel_kaggle;
                    imgOutObj.drlevel_sushrut = (Fruits)i.drlevel_sushrut;
                    imgOutObj.drlevel_byuser = dr;
                    lstImages.Add(imgOutObj);
                });

                return Ok(new
                {
                    success = 1,
                    data = lstImages
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [HttpGet]
        [ActionName("GetImageIdsByBucket")]
        public IActionResult GetImageIdsByBucket([FromQuery] Fruits dr = Fruits.All, [FromQuery] DataSource fromsushrut = DataSource.Both, string datasetids = null)
        {
            try
            {
                IEnumerable<long> intIds = new List<long>();

                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);

                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    intIds = new BucketCalculator().GetAllIdsForSpecificBucketByPredictionSourceCondition(_dbContext, dr, KaggleAndSushrutMatchedImages.Yes, userObj.id, fromsushrut, datasetids);
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    intIds = new BucketCalculator().GetAllIdsForSpecificBucketByPredictionSourceCondition(_dbContext, dr, KaggleAndSushrutMatchedImages.No, userObj.id, fromsushrut, datasetids);
                }
                else
                {
                    intIds = new BucketCalculator().GetAllIdsForSpecificBucketByPredictionSourceCondition(_dbContext, dr, KaggleAndSushrutMatchedImages.All, userObj.id, fromsushrut, datasetids);
                }

                return Ok(new
                {
                    success = 1,
                    data = intIds
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [HttpGet]
        [ActionName("GetImageIdsByBucketEx")]
        public IActionResult GetImageIdsByBucketEx([FromQuery] Fruits dr = Fruits.All, [FromQuery] DataSource fromsushrut = DataSource.Both, string datasetids = null)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);

                string strQueryDataset = !String.IsNullOrEmpty(datasetids) ? (" AND i.datasetid IN (SELECT DISTINCT datasetid from datasetmap where userid = " + userObj.id + " AND datasetid in (" + datasetids + "))")
                                                                            : (" AND i.datasetid IN (SELECT DISTINCT datasetid from datasetmap where userid = " + userObj.id + ")");

                string qry = string.Empty;
                IEnumerable<ImageOutIds> ids = null;
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    string qryPart = dr != Fruits.All ? string.Format(" AND (i.drlevel_sushrut = {0} OR iu.drlevel_byuser = {0})", dr.GetHashCode()) : string.Empty;
                    qry = string.Format("SELECT i.id FROM images i LEFT OUTER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE(iu.userid = {0} OR iu.userid IS NULL) AND i.drlevel_kaggle = i.drlevel_sushrut{1}{2};", userObj.id, qryPart, strQueryDataset);
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    string qryPart = string.Empty;
                    switch (fromsushrut)
                    {
                        case DataSource.Sushrut:
                            qryPart = dr != Fruits.All ? string.Format(" AND (i.drlevel_sushrut = {0} OR iu.drlevel_byuser = {0})", dr.GetHashCode()) : string.Empty;
                            break;
                        case DataSource.Kaggle:
                            qryPart = dr != Fruits.All ? string.Format(" AND (i.drlevel_kaggle = {0} OR iu.drlevel_byuser = {0})", dr.GetHashCode()) : string.Empty;
                            break;
                        default:
                            qryPart = string.Empty;
                            break;
                    }
                    qry = string.Format("SELECT i.id FROM images i LEFT OUTER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE(iu.userid = {0} OR iu.userid IS NULL) AND i.drlevel_kaggle <> i.drlevel_sushrut{1}{2};", userObj.id, qryPart, strQueryDataset);
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.All)
                {
                    string qryPart = dr != Fruits.All ? string.Format(" AND (i.drlevel_sushrut = {0} OR i.drlevel_kaggle = {0})", dr.GetHashCode()) : string.Empty;
                    qry = string.Format("SELECT i.id FROM images i LEFT OUTER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE(iu.userid = {0} OR iu.userid IS NULL){1}{2};", userObj.id, qryPart, strQueryDataset);
                }
                ids = _dbContext.ExecuteQuery<ImageOutIds>(qry);

                List<long> intIds = new List<long>();
                if (ids != null && ids.Count() > default(int))
                {
                    ids.ToList().ForEach(i =>
                    {
                        intIds.Add(i.id);
                    });
                }

                return Ok(new
                {
                    success = 1,
                    data = intIds
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [HttpGet]
        [ActionName("GetLastSubmittedImageByBucket")]
        public IActionResult GetLastSubmittedImageByBucket([FromQuery] Fruits dr = Fruits.All, string datasetids = null)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);

                string strQueryDataset = !String.IsNullOrEmpty(datasetids) ? (" AND i.datasetid IN (SELECT DISTINCT datasetid from datasetmap where userid = " + userObj.id + " AND datasetid in (" + datasetids + "))")
                                                                            : (" AND i.datasetid IN (SELECT DISTINCT datasetid from datasetmap where userid = " + userObj.id + ")");

                List<DatasetMap> lstDatasetMap = new BucketCalculator().GetDatasetMapping(_dbContext, userObj.id, datasetids);
                int[] arrDatasetIds = lstDatasetMap.Select(i => i.datasetid).ToArray();


                string qry = string.Empty;
                IEnumerable<ImageOutIds> ids = null;
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    string qryPart = dr != Fruits.All ? string.Format(" AND iu.kaggle_sushrut_drmatched = 1", dr.GetHashCode()) : string.Empty;
                    //qry = string.Format("SELECT i.id FROM images i WHERE i.imagename = (SELECT iu.imagename FROM imagedrbyusers iu WHERE iu.userid = {0} AND iu.drlevel_byuser = {1}{2} ORDER BY iu.createdon DESC LIMIT 1);", userObj.id, dr.GetHashCode(), qryPart);
                    qry = string.Format("SELECT i.id FROM images i INNER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE iu.userid = {0} AND iu.drlevel_byuser = {1}{2}{3} ORDER BY iu.createdon DESC LIMIT 1;", userObj.id, dr.GetHashCode(), qryPart, strQueryDataset);
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    string qryPart = dr != Fruits.All ? string.Format(" AND iu.kaggle_sushrut_drmatched = 0", dr.GetHashCode()) : string.Empty;
                    //qry = string.Format("SELECT i.id FROM images i WHERE i.imagename = (SELECT iu.imagename FROM imagedrbyusers iu WHERE iu.userid = {0} AND iu.drlevel_byuser = {1}{2} ORDER BY iu.createdon DESC LIMIT 1);", userObj.id, dr.GetHashCode(), qryPart);
                    qry = string.Format("SELECT i.id FROM images i INNER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE iu.userid = {0} AND iu.drlevel_byuser = {1}{2}{3} ORDER BY iu.createdon DESC LIMIT 1;", userObj.id, dr.GetHashCode(), qryPart, strQueryDataset);
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.All)
                {
                    string qryPart = string.Empty;
                    //qry = string.Format("SELECT i.id FROM images i WHERE i.imagename = (SELECT iu.imagename FROM imagedrbyusers iu WHERE iu.userid = {0} AND iu.drlevel_byuser = {1}{2} ORDER BY iu.createdon DESC LIMIT 1);", userObj.id, dr.GetHashCode(), qryPart);
                    qry = string.Format("SELECT i.id FROM images i INNER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE iu.userid = {0} AND iu.drlevel_byuser = {1}{2}{3} ORDER BY iu.createdon DESC LIMIT 1;", userObj.id, dr.GetHashCode(), qryPart, strQueryDataset);
                }
                ids = _dbContext.ExecuteQuery<ImageOutIds>(qry);

                long? intId = null;
                if (ids != null && ids.Count() > default(int))
                {
                    intId = ids.ToList().FirstOrDefault().id;
                }

                return Ok(new
                {
                    success = 1,
                    data = intId
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [HttpGet]
        [ActionName("GetSingleImage")]
        public IActionResult GetSingleImage([FromQuery] long id)
        {
            ImageOutWithAnnotation obj = new ImageOutWithAnnotation();
            try
            {
                if (id <= default(long))
                {
                    throw new Exception("Parameter not found.");
                }

                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);

                ImageDrByUser userGradedImage = null;
                Image objImage;
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    objImage = _dbContext.images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut).SingleOrDefault(i => i.id == id);

                    userGradedImage = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id && u.imagename == objImage.imagename
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()).FirstOrDefault();
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    objImage = _dbContext.images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut).SingleOrDefault(i => i.id == id);

                    userGradedImage = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id && u.imagename == objImage.imagename
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()).FirstOrDefault();
                }
                else
                {
                    objImage = _dbContext.images.SingleOrDefault(i => i.id == id);

                    userGradedImage = _dbContext.imagedrbyusers.Where(u => u.userid == userObj.id && u.imagename == objImage.imagename
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()).FirstOrDefault();
                }
                obj.id = objImage.id;
                obj.imagename = objImage.imagename;
                obj.drlevel_kaggle = (Fruits)objImage.drlevel_kaggle;
                obj.drlevel_sushrut = (Fruits)objImage.drlevel_sushrut;
                obj.drlevel_byuser = userGradedImage != null ? (Fruits)userGradedImage.drlevel_byuser : Fruits.All;
                obj.subdiseaseids = userGradedImage != null && !string.IsNullOrEmpty(userGradedImage.subdiseaseids) ? userGradedImage.subdiseaseids.Split(',').Select(int.Parse).ToList() : null;
                //obj.image = new ImageHandler(_configuration).GetFileFromLocal(objImage.imagename);
                obj.image = new ImageHandler(_configuration).GetFileFromLocal(objImage.imageurl);
                obj.regionannotation = userGradedImage != null && !string.IsNullOrEmpty(userGradedImage.regionannotation) ? GetSavedAnnotationById(userGradedImage.regionannotation) : null;
                obj.datasetid = objImage.datasetid;
                obj.superadmin = userObj.admin;
                obj.markedforreview = userGradedImage != null && !string.IsNullOrEmpty(userGradedImage.markedforreview) ? userGradedImage.markedforreview : null;
                DatasetMap dsMap = _dbContext.datasetmap.Where(ds => ds.userid == userObj.id && ds.datasetid == objImage.datasetid).FirstOrDefault();
                obj.is_admin = dsMap.isadmin;
                obj.is_anonymous = dsMap.isanonymous;
                if(obj.is_anonymous)
                {
                    obj.drlevel_kaggle = Fruits.Ungraded;
                    obj.drlevel_sushrut = Fruits.Ungraded;
                }
                else
                {
                    if (obj.superadmin || obj.is_admin)
                    {
                        obj.users_prediction = (from iu in _dbContext.imagedrbyusers
                                                join u in _dbContext.users on iu.userid equals u.id
                                                where iu.imagename == objImage.imagename
                                                where iu.userid != userObj.id
                                                orderby iu.id
                                                select new UsersPrediction()
                                                {
                                                    userid = iu.userid,
                                                    username = u.firstname + " " + u.lastname, //u.username,
                                                    predictionid = iu.id,
                                                    dr_level = (Fruits)iu.drlevel_byuser,
                                                    markedforreview = iu.markedforreview
                                                }).ToList();
                    }
                }

                return Ok(new
                {
                    success = 1,
                    data = obj
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [HttpGet]
        [ActionName("GetImagesMarkedForReview")]
        public IActionResult GetImagesMarkedForReview([FromQuery] string datasetids, [FromQuery] Fruits dr = Fruits.All)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);

                if (userObj.admin)
                {
                    if (String.IsNullOrEmpty(datasetids))
                    {
                        var result = (from i in _dbContext.images
                                      join iu in _dbContext.imagedrbyusers on i.imagename equals iu.imagename
                                      join u in _dbContext.users on iu.userid equals u.id
                                      where iu.markedforreview == MarkedForReview.requested
                                      orderby iu.id
                                      select new
                                      {
                                          predictionid = iu.id,
                                          imageid = i.id,
                                          imagename = iu.imagename,
                                          datasetid = i.datasetid,
                                          kaggle_sushrut_drmatched = iu.kaggle_sushrut_drmatched,
                                          drlevel_byuser = iu.drlevel_byuser,
                                          subdiseaseids = iu.subdiseaseids,
                                          userid = iu.userid,
                                          username = u.username,
                                          supedadmin = u.admin,
                                          createdon = iu.createdon,
                                          modifiedon = iu.modifiedon,
                                          regionannotation = iu.regionannotation,
                                          markedforreview = iu.markedforreview
                                      }).ToList();

                        return Ok(new
                        {
                            success = 1,
                            data = dr == Fruits.All ? result : result.Where(r => r.drlevel_byuser == dr.GetHashCode())
                        });
                    }
                    else
                    {
                        string[] lstDatasetIds = datasetids.Trim().Split(',');
                        var result = (from i in _dbContext.images
                                      join iu in _dbContext.imagedrbyusers on i.imagename equals iu.imagename
                                      join u in _dbContext.users on iu.userid equals u.id
                                      where lstDatasetIds.Contains(i.datasetid.ToString())
                                      where iu.markedforreview == MarkedForReview.requested
                                      orderby iu.id
                                      select new
                                      {
                                          predictionid = iu.id,
                                          imageid = i.id,
                                          imagename = iu.imagename,
                                          datasetid = i.datasetid,
                                          kaggle_sushrut_drmatched = iu.kaggle_sushrut_drmatched,
                                          drlevel_byuser = iu.drlevel_byuser,
                                          subdiseaseids = iu.subdiseaseids,
                                          userid = iu.userid,
                                          username = u.username,
                                          supedadmin = u.admin,
                                          createdon = iu.createdon,
                                          modifiedon = iu.modifiedon,
                                          regionannotation = iu.regionannotation,
                                          markedforreview = iu.markedforreview
                                      }).ToList();

                        return Ok(new
                        {
                            success = 1,
                            data = dr == Fruits.All ? result : result.Where(r => r.drlevel_byuser == dr.GetHashCode())
                        });
                    }
                }
                else
                {
                    List<DatasetMap> lstDatasetMap = new BucketCalculator().GetDatasetMapping(_dbContext, userObj.id, datasetids);
                    int[] arrDatasetIds = lstDatasetMap.Select(i => i.datasetid).ToArray();
                    var result = (from i in _dbContext.images
                                  join iu in _dbContext.imagedrbyusers on i.imagename equals iu.imagename
                                  join u in _dbContext.users on iu.userid equals u.id
                                  where arrDatasetIds.Contains(i.datasetid)
                                  where iu.markedforreview == MarkedForReview.requested
                                  orderby iu.id
                                  select new
                                  {
                                      predictionid = iu.id,
                                      imageid = i.id,
                                      imagename = iu.imagename,
                                      datasetid = i.datasetid,
                                      kaggle_sushrut_drmatched = iu.kaggle_sushrut_drmatched,
                                      drlevel_byuser = iu.drlevel_byuser,
                                      subdiseaseids = iu.subdiseaseids,
                                      userid = iu.userid,
                                      username = u.username,
                                      supedadmin = u.admin,
                                      createdon = iu.createdon,
                                      modifiedon = iu.modifiedon,
                                      regionannotation = iu.regionannotation,
                                      markedforreview = iu.markedforreview
                                  }).ToList();

                    return Ok(new
                    {
                        success = 1,
                        data = dr == Fruits.All ? result : result.Where(r => r.drlevel_byuser == dr.GetHashCode())
                    });
                }
            }
            catch (Exception)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [HttpGet]
        [ActionName("GetReviewedImages")]
        public IActionResult GetReviewedImages([FromQuery] string datasetids, [FromQuery] Fruits dr = Fruits.All)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);

                if (userObj.admin)
                {
                    if (String.IsNullOrEmpty(datasetids))
                    {
                        var result = (from i in _dbContext.images
                                      join iu in _dbContext.imagedrbyusers on i.imagename equals iu.imagename
                                      join u in _dbContext.users on iu.userid equals u.id
                                      where iu.markedforreview == MarkedForReview.reviewed
                                      orderby iu.id
                                      select new
                                      {
                                          predictionid = iu.id,
                                          imageid = i.id,
                                          imagename = iu.imagename,
                                          datasetid = i.datasetid,
                                          kaggle_sushrut_drmatched = iu.kaggle_sushrut_drmatched,
                                          drlevel_byuser = iu.drlevel_byuser,
                                          subdiseaseids = iu.subdiseaseids,
                                          userid = iu.userid,
                                          username = u.username,
                                          supedadmin = u.admin,
                                          createdon = iu.createdon,
                                          modifiedon = iu.modifiedon,
                                          regionannotation = iu.regionannotation,
                                          markedforreview = iu.markedforreview
                                      }).ToList();

                        return Ok(new
                        {
                            success = 1,
                            data = dr == Fruits.All ? result : result.Where(r => r.drlevel_byuser == dr.GetHashCode())
                        });
                    }
                    else
                    {
                        string[] lstDatasetIds = datasetids.Trim().Split(',');
                        var result = (from i in _dbContext.images
                                      join iu in _dbContext.imagedrbyusers on i.imagename equals iu.imagename
                                      join u in _dbContext.users on iu.userid equals u.id
                                      where lstDatasetIds.Contains(i.datasetid.ToString())
                                      where iu.markedforreview == MarkedForReview.reviewed
                                      orderby iu.id
                                      select new
                                      {
                                          predictionid = iu.id,
                                          imageid = i.id,
                                          imagename = iu.imagename,
                                          datasetid = i.datasetid,
                                          kaggle_sushrut_drmatched = iu.kaggle_sushrut_drmatched,
                                          drlevel_byuser = iu.drlevel_byuser,
                                          subdiseaseids = iu.subdiseaseids,
                                          userid = iu.userid,
                                          username = u.username,
                                          supedadmin = u.admin,
                                          createdon = iu.createdon,
                                          modifiedon = iu.modifiedon,
                                          regionannotation = iu.regionannotation,
                                          markedforreview = iu.markedforreview
                                      }).ToList();

                        return Ok(new
                        {
                            success = 1,
                            data = dr == Fruits.All ? result : result.Where(r => r.drlevel_byuser == dr.GetHashCode())
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        success = default(int),
                        message = "User does not have enough rights."
                    });

                    #region if we allow non-admin users to fetch images they reviewd 
                    ////List<DatasetMap> lstDatasetMap = new BucketCalculator().GetDatasetMapping(_dbContext, userObj.id, datasetids);
                    ////int[] arrDatasetIds = lstDatasetMap.Select(i => i.datasetid).ToArray();
                    ////var result = (from i in _dbContext.images
                    ////              join iu in _dbContext.imagedrbyusers on i.imagename equals iu.imagename
                    ////              join u in _dbContext.users on iu.userid equals u.id
                    ////              where arrDatasetIds.Contains(i.datasetid)
                    ////              where iu.markedforreview == MarkedForReview.reviewed
                    ////              orderby iu.id
                    ////              select new
                    ////              {
                    ////                  predictionid = iu.id,
                    ////                  imageid = i.id,
                    ////                  imagename = iu.imagename,
                    ////                  datasetid = i.datasetid,
                    ////                  kaggle_sushrut_drmatched = iu.kaggle_sushrut_drmatched,
                    ////                  drlevel_byuser = iu.drlevel_byuser,
                    ////                  subdiseaseids = iu.subdiseaseids,
                    ////                  userid = iu.userid,
                    ////                  username = u.username,
                    ////                  supedadmin = u.admin,
                    ////                  createdon = iu.createdon,
                    ////                  modifiedon = iu.modifiedon,
                    ////                  regionannotation = iu.regionannotation,
                    ////                  markedforreview = iu.markedforreview
                    ////              }).ToList();

                    ////return Ok(new
                    ////{
                    ////    success = 1,
                    ////    data = dr == DRStatus.All ? result : result.Where(r => r.drlevel_byuser == dr.GetHashCode())
                    ////}); 
                    #endregion
                }
            }
            catch (Exception)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [HttpPost]
        [ActionName("ReviewAnnotation")]
        public IActionResult ReviewAnnotation([FromBody] UpdateAnnotationMarkedForReview updateAnnotationMarkedForReview)
        {
            try
            {
                //admin only
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);

                if (!userObj.admin)
                {
                    return Ok(new
                    {
                        success = default(int),
                        message = "User does not have enough rights."
                    });
                }

                ImageDrByUser objImageDr = _dbContext.imagedrbyusers.Where(iu => iu.id == updateAnnotationMarkedForReview.predictionid).SingleOrDefault();
                if (objImageDr == null)
                {
                    return Ok(new
                    {
                        success = default(int),
                        message = "No record found for given predictionid."
                    });
                }

                string strMessage = null;
                switch (updateAnnotationMarkedForReview.action)
                {
                    case ReviewAction.Review:
                        {
                            //action = review
                            //if review == null/ reviewd  =>  review = requested
                            //if review == requested  =>  do nothing
                            if (objImageDr.markedforreview != MarkedForReview.requested)
                            {
                                objImageDr.markedforreview = MarkedForReview.requested;
                                _dbContext.SaveChanges();
                                strMessage = "Prediction has been marked for review.";
                            }
                            else
                            {
                                strMessage = "Prediction is already marked for review.";
                            }
                        }
                        break;
                    case ReviewAction.Complete:
                        {
                            //action = complete
                            //review == reviewed  =>  review = completed
                            //review == requested  =>  do nothing
                            if (objImageDr.markedforreview == MarkedForReview.reviewed)
                            {
                                objImageDr.markedforreview = MarkedForReview.completed;
                                _dbContext.SaveChanges();
                                strMessage = "Prediction has been marked as completed.";
                            }
                            else
                            {
                                strMessage = "Prediction is not in reviewed state.";
                            }
                        }
                        break;
                    case ReviewAction.Cancel:
                        {
                            //action = cancel
                            //review = null
                            objImageDr.markedforreview = null;
                            _dbContext.SaveChanges();
                            strMessage = "Review request for given prediction has been cancelled.";
                        }
                        break;
                    default:
                        {
                            //default action = review
                            //if review == null/ reviewd  =>  review = requested
                            //if review == requested  =>  do nothing
                            if (objImageDr.markedforreview != MarkedForReview.requested)
                            {
                                objImageDr.markedforreview = MarkedForReview.requested;
                                _dbContext.SaveChanges();
                                strMessage = "Prediction has been marked for review.";
                            }
                            else
                            {
                                strMessage = "Prediction is already marked for review.";
                            }
                        }
                        break;
                }
                return Ok(new
                {
                    success = 1,
                    message = strMessage
                });
            }
            catch (Exception)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [HttpPost]
        [ActionName("UpdateDrRecord")]
        public IActionResult UpdateDrRecord([FromBody] ImageDrByUserFinal objs)
        {
            try
            {
                if (objs != null)
                {
                    ImageDrByUser obj = new ImageDrByUser();
                    obj.id = objs.id;
                    obj.imagename = objs.imagename;
                    obj.kaggle_sushrut_drmatched = objs.kaggle_sushrut_drmatched;
                    obj.userid = objs.userid;
                    obj.drlevel_byuser = objs.drlevel_byuser;
                    obj.subdiseaseids = objs.subdiseaseids != null && objs.subdiseaseids.Count() > default(int) ? string.Join(",", objs.subdiseaseids.Select(n => n.ToString()).ToArray()) : null;
                    obj.createdon = objs.createdon;
                    obj.modifiedon = objs.modifiedon;
                    obj.regionannotation = objs.regionannotation == null || objs.regionannotation.markers == null ?
                        null :
                        (!string.IsNullOrEmpty(objs.regionannotation.id) ?
                        SaveOrDeleteAnnotation(objs.regionannotation, objs.regionannotation.id) :
                        SaveOrDeleteAnnotation(objs.regionannotation));
                    obj.markedforreview = objs.markedforreview;

                    var claimsIdentity = this.User.Identity as ClaimsIdentity;
                    var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                    User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);
                    obj.userid = userObj.id;
                    obj.kaggle_sushrut_drmatched = Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]);

                    if (string.IsNullOrEmpty(obj.imagename))
                    {
                        var imageObj = _dbContext.images.Where(i => i.id == obj.id).FirstOrDefault();
                        obj.imagename = imageObj.imagename;
                        obj.id = default(long);
                    }

                    ImageDrByUser imgObj = null;
                    if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                    {
                        imgObj = _dbContext.imagedrbyusers.Where(i => i.imagename == obj.imagename
                        && i.userid == obj.userid
                        && i.kaggle_sushrut_drmatched == obj.kaggle_sushrut_drmatched).FirstOrDefault();
                    }
                    else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                    {
                        imgObj = _dbContext.imagedrbyusers.Where(i => i.imagename == obj.imagename
                        && i.userid == obj.userid
                        && i.kaggle_sushrut_drmatched != obj.kaggle_sushrut_drmatched).FirstOrDefault();
                    }
                    else
                    {
                        imgObj = _dbContext.imagedrbyusers.Where(i => i.imagename == obj.imagename
                        && i.userid == obj.userid).FirstOrDefault();
                    }

                    if (imgObj != null)
                    {
                        // update
                        imgObj.modifiedon = DateTime.Now;
                        //imgObj.kaggle_sushrut_drmatched = obj.kaggle_sushrut_drmatched;
                        imgObj.drlevel_byuser = obj.drlevel_byuser;
                        imgObj.subdiseaseids = obj.subdiseaseids;
                        if (imgObj.markedforreview != null)
                        {
                            imgObj.markedforreview = MarkedForReview.reviewed;
                        }
                        if (!string.IsNullOrEmpty(imgObj.regionannotation))
                        {
                            if (string.IsNullOrEmpty(obj.regionannotation))
                            {
                                try
                                {
                                    SaveOrDeleteAnnotation(null, imgObj.regionannotation, true);
                                }
                                catch
                                { }
                                finally
                                {
                                    imgObj.regionannotation = null;
                                }
                            }
                            else
                            {
                                imgObj.regionannotation = obj.regionannotation;
                            }
                        }
                        else
                        {
                            imgObj.regionannotation = obj.regionannotation;
                        }
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        // insert
                        obj.createdon = DateTime.Now;
                        _dbContext.imagedrbyusers.Add(obj);
                        _dbContext.SaveChanges();
                    }

                    return Ok(new
                    {
                        success = 1,
                        message = string.Format("Prediction has been {0} successfully.", (imgObj != null ? "updated" : "added"))
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = default(int),
                        message = "No data has been supplied."
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = default(int),
                    message = "Exception has been detected. Please contact to the authority."
                });
            }
        }

        [NonAction]
        private AnnotationObject GetSavedAnnotationById(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return _annotationService.GetById(id);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        [NonAction]
        private string SaveOrDeleteAnnotation(AnnotationObject obj, string id = "", bool delete = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var annoObj = _annotationService.GetById(id);
                    if (annoObj != null)
                    {
                        if (!delete)
                        {
                            _annotationService.Update(id, obj);
                            return id;
                        }
                        else
                        {
                            _annotationService.Delete(annoObj.id);
                            return annoObj.id;
                        }
                    }
                    else
                    {
                        var x = _annotationService.Create(obj);
                        return x.id;
                    }
                }
                else
                {
                    var x = _annotationService.Create(obj);
                    return x.id;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
