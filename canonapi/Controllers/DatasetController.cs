using canonapi.Authentication;
using canonapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace canonapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DatasetController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public DatasetController(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbContext = context;
        }

        [HttpPost]
        [ActionName("CreateDataset")]
        public IActionResult CreateDataset([FromBody] Datasets objSource)
        {
            try
            {
                User objUser = GetUser();
                if (!objUser.admin)
                {
                    return Ok(new
                    {
                        success = default(int),
                        message = "User does not have enough rights"
                    });
                }
                if (objSource != null)
                {
                    if (String.IsNullOrEmpty(objSource.adminid))
                    {
                        objSource.adminid = objUser.id.ToString();
                    }
                    _dbContext.datasets.Add(objSource);
                    _dbContext.SaveChanges();

                    DatasetMap objDatasetMap = new DatasetMap() { 
                        id = 0, 
                        userid = objUser.id, 
                        datasetid = _dbContext.datasets.Where(d => d.datasetname == objSource.datasetname).Select(d => d.id).FirstOrDefault(), 
                        isadmin = true 
                    };
                    _dbContext.datasetmap.Add(objDatasetMap);
                    _dbContext.SaveChanges();
                }

                return Ok(new
                {
                    success = 1,
                    message = "Success"
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
        [ActionName("UpdateDataset")]
        public IActionResult UpdateDataset([FromBody] Datasets objs)
        {
            try
            {
                if (!GetUser().admin)
                {
                    return Ok(new
                    {
                        success = default(int),
                        message = "User does not have enough rights"
                    });
                }
                if (objs != null)
                {
                    Datasets objds = null;
                    objds = _dbContext.datasets.Where(d => d.datasetname == objs.datasetname).FirstOrDefault();
                    if (objds != null)
                    {
                        objds.description = objs.description;
                        objds.attribute = objs.attribute;
                        objds.referenceid = objs.referenceid;
                        objds.adminid = objs.adminid;

                        _dbContext.SaveChanges();


                        return Ok(new
                        {
                            success = 1,
                            message = "Success"
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = 0,
                            message = "No matching dataset found for the given name"
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        success = 0,
                        message = "invalid inputs"
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

        [HttpPost]
        [ActionName("DeleteDataset")]
        public IActionResult DeleteDataset([FromBody] Datasets objs)
        {
            try
            {
                if (!GetUser().admin)
                {
                    return Ok(new
                    {
                        success = default(int),
                        message = "User does not have enough rights"
                    });
                }
                if (objs != null)
                {
                    _dbContext.datasets.Remove(objs);
                    _dbContext.SaveChanges();

                    return Ok(new
                    {
                        success = 1,
                        message = "Success"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = 0,
                        message = "No matching dataset found"
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
        private User GetUser()
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);
                return userObj;
            }
            catch (Exception ex)
            {
                return new User() { admin = false };
            }
        }
    }
}
