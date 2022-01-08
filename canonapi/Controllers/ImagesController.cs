using canonapi.Authentication;
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
        public IActionResult GetImageCount()
        {
            UserWisePredictionCounts counts = new UserWisePredictionCounts();
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.Users.SingleOrDefault(u => u.username == username);

                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    counts.totalImages = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut).Count();
                    counts.totalImagesPredicted = _dbContext.ImageDrByUsers.Where(i => i.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()).Count();
                    //DR0 Count
                    {
                        counts.totalDR0FromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            DRStatus.DR0, KaggleAndSushrutMatchedImages.Yes, userObj.id);
                    }
                    //DR1 Count
                    {
                        counts.totalDR1FromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            DRStatus.DR1, KaggleAndSushrutMatchedImages.Yes, userObj.id);
                    }
                    //DR2 Count
                    {
                        counts.totalDR2FromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            DRStatus.DR2, KaggleAndSushrutMatchedImages.Yes, userObj.id);
                    }
                    //DR3 Count
                    {
                        counts.totalDR3FromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            DRStatus.DR3, KaggleAndSushrutMatchedImages.Yes, userObj.id);
                    }
                    //DR4 Count
                    {
                        counts.totalDR4FromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            DRStatus.DR4, KaggleAndSushrutMatchedImages.Yes, userObj.id);
                    }
                    //Others Count
                    {
                        counts.totalOthersFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            DRStatus.Others, KaggleAndSushrutMatchedImages.Yes, userObj.id);
                    }
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    counts.totalImages = _dbContext.Images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut).Count();
                    counts.totalImagesPredicted = _dbContext.ImageDrByUsers.Where(i => i.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()).Count();
                    //DR0 Count
                    {
                        counts.totalDR0FromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            DRStatus.DR0, KaggleAndSushrutMatchedImages.No, userObj.id);
                    }
                    //DR1 Count
                    {
                        counts.totalDR1FromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            DRStatus.DR1, KaggleAndSushrutMatchedImages.No, userObj.id);
                    }
                    //DR2 Count
                    {
                        counts.totalDR2FromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            DRStatus.DR2, KaggleAndSushrutMatchedImages.No, userObj.id);
                    }
                    //DR3 Count
                    {
                        counts.totalDR3FromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            DRStatus.DR3, KaggleAndSushrutMatchedImages.No, userObj.id);
                    }
                    //DR4 Count
                    {
                        counts.totalDR4FromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            DRStatus.DR4, KaggleAndSushrutMatchedImages.No, userObj.id);
                    }
                    //Others Count
                    {
                        counts.totalOthersFromPredicted = new BucketCalculator().CalculateSpecificBucketByCondition(_dbContext,
                            DRStatus.Others, KaggleAndSushrutMatchedImages.No, userObj.id);
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
        public IActionResult GetImageCountEx()
        {
            UserWisePredictionCounts counts = new UserWisePredictionCounts();
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.Users.SingleOrDefault(u => u.username == username);

                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    counts.totalImagesPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()).Count();

                    counts.totalDR0FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR0.GetHashCode()).Count();

                    counts.totalDR1FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR1.GetHashCode()).Count();

                    counts.totalDR2FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR2.GetHashCode()).Count();

                    counts.totalDR3FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR3.GetHashCode()).Count();

                    counts.totalDR4FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR4.GetHashCode()).Count();

                    counts.totalOthersFromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                    && u.drlevel_byuser == DRStatus.Others.GetHashCode()).Count();

                    counts.totalImages = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut).Count();
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    counts.totalImagesPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()).Count();

                    counts.totalDR0FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR0.GetHashCode()).Count();

                    counts.totalDR1FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR1.GetHashCode()).Count();

                    counts.totalDR2FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR2.GetHashCode()).Count();

                    counts.totalDR3FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR3.GetHashCode()).Count();

                    counts.totalDR4FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR4.GetHashCode()).Count();

                    counts.totalOthersFromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                    && u.drlevel_byuser == DRStatus.Others.GetHashCode()).Count();

                    counts.totalImages = _dbContext.Images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut).Count();
                }
                else
                {
                    counts.totalImagesPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()).Count();

                    counts.totalDR0FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR0.GetHashCode()).Count();

                    counts.totalDR1FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR1.GetHashCode()).Count();

                    counts.totalDR2FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR2.GetHashCode()).Count();

                    counts.totalDR3FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR3.GetHashCode()).Count();

                    counts.totalDR4FromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == DRStatus.DR4.GetHashCode()).Count();

                    counts.totalOthersFromPredicted = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                    && u.drlevel_byuser == DRStatus.Others.GetHashCode()).Count();

                    counts.totalImages = _dbContext.Images.Count();
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
        public IActionResult GetImageCountByBucket()
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.Users.SingleOrDefault(u => u.username == username);

                List<CountMaster> counts = new List<CountMaster>();
                int xKaggleDR0, xKaggleDR1, xKaggleDR2, xKaggleDR3, xKaggleDR4, xKaggleOthers, xSushrutDR0, xSushrutDR1, xSushrutDR2, xSushrutDR3, xSushrutDR4, xSushrutOthers;
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    xKaggleDR0 = xSushrutDR0 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR0, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut);

                    xKaggleDR1 = xSushrutDR1 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR1, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut);

                    xKaggleDR2 = xSushrutDR2 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR2, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut);

                    xKaggleDR3 = xSushrutDR3 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR3, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut);

                    xKaggleDR4 = xSushrutDR4 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR4, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut);

                    xKaggleOthers = xSushrutOthers = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.Others, KaggleAndSushrutMatchedImages.Yes, userObj.id, DataSource.Sushrut);

                    counts.Add(new CountMaster()
                    {
                        Kaggle_DR0 = xKaggleDR0,
                        Kaggle_DR1 = xKaggleDR1,
                        Kaggle_DR2 = xKaggleDR2,
                        Kaggle_DR3 = xKaggleDR3,
                        Kaggle_DR4 = xKaggleDR4,
                        Kaggle_Others = xKaggleOthers,
                        Sushrut_DR0 = xSushrutDR0,
                        Sushrut_DR1 = xSushrutDR1,
                        Sushrut_DR2 = xSushrutDR2,
                        Sushrut_DR3 = xSushrutDR3,
                        Sushrut_DR4 = xSushrutDR4,
                        Sushrut_Others = xSushrutOthers
                    });
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    xKaggleDR0 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR0, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle);
                    xSushrutDR0 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR0, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut);

                    xKaggleDR1 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR1, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle);
                    xSushrutDR1 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR1, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut);

                    xKaggleDR2 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR2, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle);
                    xSushrutDR2 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR2, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut);

                    xKaggleDR3 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR3, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle);
                    xSushrutDR3 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR3, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut);

                    xKaggleDR4 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR4, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle);
                    xSushrutDR4 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR4, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut);

                    xKaggleOthers = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.Others, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Kaggle);
                    xSushrutOthers = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.Others, KaggleAndSushrutMatchedImages.No, userObj.id, DataSource.Sushrut);

                    counts.Add(new CountMaster()
                    {
                        Kaggle_DR0 = xKaggleDR0,
                        Kaggle_DR1 = xKaggleDR1,
                        Kaggle_DR2 = xKaggleDR2,
                        Kaggle_DR3 = xKaggleDR3,
                        Kaggle_DR4 = xKaggleDR4,
                        Kaggle_Others = xKaggleOthers,
                        Sushrut_DR0 = xSushrutDR0,
                        Sushrut_DR1 = xSushrutDR1,
                        Sushrut_DR2 = xSushrutDR2,
                        Sushrut_DR3 = xSushrutDR3,
                        Sushrut_DR4 = xSushrutDR4,
                        Sushrut_Others = xSushrutOthers
                    });
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.All)
                {
                    xKaggleDR0 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR0, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle);
                    xSushrutDR0 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR0, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut);

                    xKaggleDR1 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR1, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle);
                    xSushrutDR1 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR1, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut);

                    xKaggleDR2 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR2, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle);
                    xSushrutDR2 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR2, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut);

                    xKaggleDR3 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR3, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle);
                    xSushrutDR3 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR3, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut);

                    xKaggleDR4 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR4, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle);
                    xSushrutDR4 = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.DR4, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut);

                    xKaggleOthers = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.Others, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Kaggle);
                    xSushrutOthers = new BucketCalculator().CalculateSpecificBucketByPredictionSourceCondition(_dbContext, DRStatus.Others, KaggleAndSushrutMatchedImages.All, userObj.id, DataSource.Sushrut);

                    counts.Add(new CountMaster()
                    {
                        Kaggle_DR0 = xKaggleDR0,
                        Kaggle_DR1 = xKaggleDR1,
                        Kaggle_DR2 = xKaggleDR2,
                        Kaggle_DR3 = xKaggleDR3,
                        Kaggle_DR4 = xKaggleDR4,
                        Kaggle_Others = xKaggleOthers,
                        Sushrut_DR0 = xSushrutDR0,
                        Sushrut_DR1 = xSushrutDR1,
                        Sushrut_DR2 = xSushrutDR2,
                        Sushrut_DR3 = xSushrutDR3,
                        Sushrut_DR4 = xSushrutDR4,
                        Sushrut_Others = xSushrutOthers
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
        public IActionResult GetImageCountByBucketEx()
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.Users.SingleOrDefault(u => u.username == username);

                IEnumerable<CountMaster> counts = new List<CountMaster>();
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    counts = _dbContext.ExecuteQuery<CountMaster>(string.Format(QueryHelper.qryMatchedResultCount, userObj.id));
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    counts = _dbContext.ExecuteQuery<CountMaster>(string.Format(QueryHelper.qryUnmatchedResultCount, userObj.id));
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.All)
                {
                    counts = _dbContext.ExecuteQuery<CountMaster>(string.Format(QueryHelper.qryAllResultCount, userObj.id));
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
        public IActionResult GetPaginated([FromQuery] int page, [FromQuery] int limit, [FromQuery] DRStatus dr = DRStatus.All)
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
                User userObj = _dbContext.Users.SingleOrDefault(u => u.username == username);

                var skip = (page - 1) * limit;
                IEnumerable<Image> savedSearches = null;
                IEnumerable<ImageDrByUser> userGradedImages = null;
                IEnumerable<ImageDrByUser> userGradedImagesConditional = null;
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    if (dr == DRStatus.All)
                    {
                        userGradedImagesConditional = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                        && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode());

                        if (userGradedImagesConditional != null && userGradedImagesConditional.Count() > default(int))
                        {
                            var totRec = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut).ToList();
                            var ugradedImg = userGradedImagesConditional.ToList();
                            savedSearches = (from x in totRec where !ugradedImg.Any(y => y.imagename == x.imagename) select x).ToList().Skip(skip).Take(limit);
                        }
                        else
                        {
                            savedSearches = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut).Skip(skip).Take(limit);
                        }
                    }
                    else
                    {
                        userGradedImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                        && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()
                        && u.drlevel_byuser == dr.GetHashCode());
                    }
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    if (dr == DRStatus.All)
                    {
                        userGradedImagesConditional = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                        && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode());

                        if (userGradedImagesConditional != null && userGradedImagesConditional.Count() > default(int))
                        {
                            var totRec = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut).ToList();
                            var ugradedImg = userGradedImagesConditional.ToList();
                            savedSearches = (from x in totRec where !ugradedImg.Any(y => y.imagename != x.imagename) select x).ToList().Skip(skip).Take(limit);
                        }
                        else
                        {
                            savedSearches = _dbContext.Images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut).Skip(skip).Take(limit);
                        }
                    }
                    else
                    {
                        userGradedImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                        && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()
                        && u.drlevel_byuser == dr.GetHashCode());
                    }
                }
                else
                {
                    if (dr == DRStatus.All)
                    {
                        userGradedImagesConditional = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                        && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode());

                        if (userGradedImagesConditional != null && userGradedImagesConditional.Count() > default(int))
                        {
                            var totRec = _dbContext.Images.ToList();
                            var ugradedImg = userGradedImagesConditional.ToList();
                            savedSearches = (from x in totRec where !ugradedImg.Any(y => y.imagename == x.imagename) select x).ToList().Skip(skip).Take(limit);
                        }
                        else
                        {
                            savedSearches = _dbContext.Images.Skip(skip).Take(limit);
                        }
                    }
                    else
                    {
                        userGradedImages = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id
                        && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()
                        && u.drlevel_byuser == dr.GetHashCode());
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
                            var qryImg = _dbContext.Images.Where(i => i.imagename == ui.imagename).FirstOrDefault();
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
                    imgOutObj.thumbnail = new ImageHandler(_configuration).GetFileFromLocal(i.imagename, true);
                    imgOutObj.drlevel_kaggle = (DRStatus)i.drlevel_kaggle;
                    imgOutObj.drlevel_sushrut = (DRStatus)i.drlevel_sushrut;
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
        public IActionResult GetImageIdsByBucket([FromQuery] DRStatus dr = DRStatus.All, [FromQuery] DataSource fromsushrut = DataSource.Both)
        {
            try
            {
                IEnumerable<long> intIds = new List<long>();

                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.Users.SingleOrDefault(u => u.username == username);

                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    intIds = new BucketCalculator().GetAllIdsForSpecificBucketByPredictionSourceCondition(_dbContext, dr, KaggleAndSushrutMatchedImages.Yes, userObj.id, fromsushrut);
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    intIds = new BucketCalculator().GetAllIdsForSpecificBucketByPredictionSourceCondition(_dbContext, dr, KaggleAndSushrutMatchedImages.No, userObj.id, fromsushrut);
                }
                else
                {
                    intIds = new BucketCalculator().GetAllIdsForSpecificBucketByPredictionSourceCondition(_dbContext, dr, KaggleAndSushrutMatchedImages.All, userObj.id, fromsushrut);
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
        public IActionResult GetImageIdsByBucketEx([FromQuery] DRStatus dr = DRStatus.All, [FromQuery] DataSource fromsushrut = DataSource.Both)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.Users.SingleOrDefault(u => u.username == username);

                string qry = string.Empty;
                IEnumerable<ImageOutIds> ids = null;
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    string qryPart = dr != DRStatus.All ? string.Format(" AND (i.drlevel_sushrut = {0} OR iu.drlevel_byuser = {0})", dr.GetHashCode()) : string.Empty;
                    qry = string.Format("SELECT i.id FROM images i LEFT OUTER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE(iu.userid = {0} OR iu.userid IS NULL) AND i.drlevel_kaggle = i.drlevel_sushrut{1};", userObj.id, qryPart);
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    string qryPart = string.Empty;
                    switch (fromsushrut)
                    {
                        case DataSource.Sushrut:
                            qryPart = dr != DRStatus.All ? string.Format(" AND (i.drlevel_sushrut = {0} OR iu.drlevel_byuser = {0})", dr.GetHashCode()) : string.Empty;
                            break;
                        case DataSource.Kaggle:
                            qryPart = dr != DRStatus.All ? string.Format(" AND (i.drlevel_kaggle = {0} OR iu.drlevel_byuser = {0})", dr.GetHashCode()) : string.Empty;
                            break;
                        default:
                            qryPart = string.Empty;
                            break;
                    }
                    qry = string.Format("SELECT i.id FROM images i LEFT OUTER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE(iu.userid = {0} OR iu.userid IS NULL) AND i.drlevel_kaggle <> i.drlevel_sushrut{1};", userObj.id, qryPart);
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.All)
                {
                    string qryPart = dr != DRStatus.All ? string.Format(" AND (i.drlevel_sushrut = {0} OR i.drlevel_kaggle = {0})", dr.GetHashCode()) : string.Empty;
                    qry = string.Format("SELECT i.id FROM images i LEFT OUTER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE(iu.userid = {0} OR iu.userid IS NULL){1};", userObj.id, qryPart);
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
        public IActionResult GetLastSubmittedImageByBucket([FromQuery] DRStatus dr = DRStatus.All)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.Users.SingleOrDefault(u => u.username == username);

                string qry = string.Empty;
                IEnumerable<ImageOutIds> ids = null;
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    string qryPart = dr != DRStatus.All ? string.Format(" AND iu.kaggle_sushrut_drmatched = 1", dr.GetHashCode()) : string.Empty;
                    qry = string.Format("SELECT i.id FROM images i WHERE i.imagename = (SELECT iu.imagename FROM imagedrbyusers iu WHERE iu.userid = {0} AND iu.drlevel_byuser = {1}{2} ORDER BY iu.createdon DESC LIMIT 1);", userObj.id, dr.GetHashCode(), qryPart);
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    string qryPart = dr != DRStatus.All ? string.Format(" AND iu.kaggle_sushrut_drmatched = 0", dr.GetHashCode()) : string.Empty;
                    qry = string.Format("SELECT i.id FROM images i WHERE i.imagename = (SELECT iu.imagename FROM imagedrbyusers iu WHERE iu.userid = {0} AND iu.drlevel_byuser = {1}{2} ORDER BY iu.createdon DESC LIMIT 1);", userObj.id, dr.GetHashCode(), qryPart);
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.All)
                {
                    string qryPart = string.Empty;
                    qry = string.Format("SELECT i.id FROM images i WHERE i.imagename = (SELECT iu.imagename FROM imagedrbyusers iu WHERE iu.userid = {0} AND iu.drlevel_byuser = {1}{2} ORDER BY iu.createdon DESC LIMIT 1);", userObj.id, dr.GetHashCode(), qryPart);
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
                User userObj = _dbContext.Users.SingleOrDefault(u => u.username == username);

                ImageDrByUser userGradedImage = null;
                Image objImage;
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    objImage = _dbContext.Images.Where(i => i.drlevel_kaggle == i.drlevel_sushrut).SingleOrDefault(i => i.id == id);

                    userGradedImage = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id && u.imagename == objImage.imagename
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.Yes.GetHashCode()).FirstOrDefault();
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    objImage = _dbContext.Images.Where(i => i.drlevel_kaggle != i.drlevel_sushrut).SingleOrDefault(i => i.id == id);

                    userGradedImage = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id && u.imagename == objImage.imagename
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.No.GetHashCode()).FirstOrDefault();
                }
                else
                {
                    objImage = _dbContext.Images.SingleOrDefault(i => i.id == id);

                    userGradedImage = _dbContext.ImageDrByUsers.Where(u => u.userid == userObj.id && u.imagename == objImage.imagename
                    && u.kaggle_sushrut_drmatched == KaggleAndSushrutMatchedImages.All.GetHashCode()).FirstOrDefault();
                }
                obj.id = objImage.id;
                obj.imagename = objImage.imagename;
                obj.drlevel_kaggle = (DRStatus)objImage.drlevel_kaggle;
                obj.drlevel_sushrut = (DRStatus)objImage.drlevel_sushrut;
                obj.drlevel_byuser = userGradedImage != null ? (DRStatus)userGradedImage.drlevel_byuser : DRStatus.All;
                obj.subdiseaseids = userGradedImage != null && !string.IsNullOrEmpty(userGradedImage.subdiseaseids) ? userGradedImage.subdiseaseids.Split(',').Select(int.Parse).ToList() : null;
                obj.image = new ImageHandler(_configuration).GetFileFromLocal(objImage.imagename);
                obj.regionannotation = userGradedImage != null && !string.IsNullOrEmpty(userGradedImage.regionannotation) ? GetSavedAnnotationById(userGradedImage.regionannotation) : null;

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

                    var claimsIdentity = this.User.Identity as ClaimsIdentity;
                    var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                    User userObj = _dbContext.Users.SingleOrDefault(u => u.username == username);
                    obj.userid = userObj.id;
                    obj.kaggle_sushrut_drmatched = Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]);

                    if (string.IsNullOrEmpty(obj.imagename))
                    {
                        var imageObj = _dbContext.Images.Where(i => i.id == obj.id).FirstOrDefault();
                        obj.imagename = imageObj.imagename;
                        obj.id = default(long);
                    }

                    ImageDrByUser imgObj = null;
                    if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                    {
                        imgObj = _dbContext.ImageDrByUsers.Where(i => i.imagename == obj.imagename
                        && i.userid == obj.userid
                        && i.kaggle_sushrut_drmatched == obj.kaggle_sushrut_drmatched).FirstOrDefault();
                    }
                    else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                    {
                        imgObj = _dbContext.ImageDrByUsers.Where(i => i.imagename == obj.imagename
                        && i.userid == obj.userid
                        && i.kaggle_sushrut_drmatched != obj.kaggle_sushrut_drmatched).FirstOrDefault();
                    }
                    else
                    {
                        imgObj = _dbContext.ImageDrByUsers.Where(i => i.imagename == obj.imagename
                        && i.userid == obj.userid).FirstOrDefault();
                    }

                    if (imgObj != null)
                    {
                        // update
                        imgObj.modifiedon = DateTime.Now;
                        //imgObj.kaggle_sushrut_drmatched = obj.kaggle_sushrut_drmatched;
                        imgObj.drlevel_byuser = obj.drlevel_byuser;
                        imgObj.subdiseaseids = obj.subdiseaseids;
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
                        _dbContext.ImageDrByUsers.Add(obj);
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
