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
            try
            {
                var totalImages = _dbContext.Images.Count();
                return Ok(new
                {
                    success = 1,
                    data = totalImages
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
        public IActionResult GetPaginated([FromQuery] int page, [FromQuery] int limit)
        {
            List<ImageOut> lstImages = new List<ImageOut>();
            try
            {
                if (page == 0)
                    page = 1;

                if (limit == 0)
                    limit = int.MaxValue;

                var skip = (page - 1) * limit;

                var savedSearches = _dbContext.Images.Skip(skip).Take(limit);
                savedSearches.ToList().ForEach(i =>
                {
                    var imgOutObj = new ImageOut();
                    imgOutObj.id = i.id;
                    imgOutObj.thumbnail = new ImageHandler(_configuration).GetFileFromLocal(i.imagename, true);
                    imgOutObj.drlevel_kaggle = (DRStatus) i.drlevel_kaggle;
                    imgOutObj.drlevel_sushrut = (DRStatus)i.drlevel_sushrut;
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
                Image objImage = _dbContext.Images.SingleOrDefault(i => i.id == id);
                obj.id = objImage.id;
                obj.drlevel_kaggle = (DRStatus)objImage.drlevel_kaggle;
                obj.drlevel_sushrut = (DRStatus)objImage.drlevel_sushrut;
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
        [ActionName("UpdateRecord")]
        public IActionResult UpdateRecord([FromBody] Image obj)
        {
            try
            {
                if (obj != null)
                {

                }
                else
                {
                    return Ok(new
                    {
                        success = default(int),
                        message = "No data has been supplied."
                    });
                }
                return null;
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
