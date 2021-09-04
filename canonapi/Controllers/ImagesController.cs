using canonapi.Authentication;
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

        public ImagesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbContext = context;
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
                /*var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.Users.SingleOrDefault(u => u.username == username);*/

                IEnumerable<CountMaster> counts = new List<CountMaster>();
                if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.Yes)
                {
                    counts = _dbContext.ExecuteQuery<CountMaster>(QueryHelper.qryMatchedResultCount);
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.No)
                {
                    counts = _dbContext.ExecuteQuery<CountMaster>(QueryHelper.qryUnmatchedResultCount);
                }
                else if ((KaggleAndSushrutMatchedImages)Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]) == KaggleAndSushrutMatchedImages.All)
                {
                    counts = _dbContext.ExecuteQuery<CountMaster>(QueryHelper.qryAllResultCount);
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
        [ActionName("GetSingleImage")]
        public IActionResult GetSingleImage([FromQuery] long id)
        {
            ImageOut obj = new ImageOut();
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
                obj.drlevel_kaggle = (DRStatus)objImage.drlevel_kaggle;
                obj.drlevel_sushrut = (DRStatus)objImage.drlevel_sushrut;
                obj.drlevel_byuser = userGradedImage != null ? (DRStatus)userGradedImage.drlevel_byuser : DRStatus.All;
                obj.image = new ImageHandler(_configuration).GetFileFromLocal(objImage.imagename);

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
        public IActionResult UpdateDrRecord([FromBody] ImageDrByUser obj)
        {
            try
            {
                if (obj != null)
                {
                    var claimsIdentity = this.User.Identity as ClaimsIdentity;
                    var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                    User userObj = _dbContext.Users.SingleOrDefault(u => u.username == username);
                    obj.userid = userObj.id;
                    obj.kaggle_sushrut_drmatched = Convert.ToInt32(_configuration["KaggleAndSushrutMatchedImages"]);

                    ImageDrByUser imgObj = _dbContext.ImageDrByUsers.Where(i => i.imagename == obj.imagename
                    && i.userid == obj.userid
                    && i.kaggle_sushrut_drmatched == obj.kaggle_sushrut_drmatched).FirstOrDefault();
                    if (imgObj != null)
                    {
                        // update
                        imgObj.modifiedon = DateTime.Now;
                        //imgObj.kaggle_sushrut_drmatched = obj.kaggle_sushrut_drmatched;
                        imgObj.drlevel_byuser = obj.drlevel_byuser;
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
    }
}
