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
    public class UserController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbContext = context;
        }

        [HttpPost]
        [ActionName("CreateUser")]
        public IActionResult CreateUser([FromBody] User objSource)
        {
            try
            {
                if (!isAuthorisedUser())
                {
                    return Ok(new
                    {
                        success = default(int),
                        message = "User does not have enough rights"
                    });
                }
                if (objSource != null)
                {
                    _dbContext.users.Add(objSource);
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
        [ActionName("UpdateUser")]
        public IActionResult UpdateUser([FromBody] User objs)
        {
            try
            {
                if (!isAuthorisedUser())
                {
                    return Ok(new
                    {
                        success = default(int),
                        message = "User does not have enough rights"
                    });
                }
                if (objs != null)
                {
                    User objUser = null;
                    objUser = _dbContext.users.Where(d => d.username == objs.username).FirstOrDefault();
                    if (objUser != null)
                    {
                        objUser.userpassword = objs.userpassword;
                        objUser.firstname = objs.firstname;
                        objUser.lastname = objs.lastname;
                        objUser.admin = objs.admin;

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
                            message = "No matching user found for the given name"
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
        [ActionName("DeleteUser")]
        public IActionResult DeleteUser([FromBody] User objs)
        {
            try
            {
                if (!isAuthorisedUser())
                {
                    return Ok(new
                    {
                        success = default(int),
                        message = "User does not have enough rights"
                    });
                }
                if (objs != null)
                {
                    _dbContext.users.Remove(objs);
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

        [HttpPost]
        [ActionName("AddOrUpdateDatasetToUser")]
        public IActionResult AddOrUpdateDatasetToUser([FromBody] List<DatasetMap> lstDatasetMap)
        {
            try
            {
                foreach (DatasetMap datasetMap in lstDatasetMap)
                {
                    //if(_dbContext.datasetmap.Any(ds => ds.userid == datasetMap.userid && ds.userid == datasetMap.datasetid))
                    //{ }
                    DatasetMap objDsMap = null;
                    objDsMap = _dbContext.datasetmap.Where(ds => ds.userid == datasetMap.userid && ds.userid == datasetMap.datasetid).First();
                    if (objDsMap == null)
                    {
                        _dbContext.datasetmap.Add(datasetMap);
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        objDsMap.isadmin = datasetMap.isadmin;
                        _dbContext.SaveChanges();
                    }

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

        [HttpGet]
        [ActionName("GetDatasetForUser")]
        public IActionResult GetDatasetForUser([FromQuery] string username)
        {
            try
            {
                List<Datasets> objDatasets = null;
                if (!String.IsNullOrEmpty(username))
                {
                    int userid = _dbContext.users.Where(u => u.username == username).Select(u => u.id).FirstOrDefault();
                    int[] datasetIds = _dbContext.datasetmap.Where(m => m.userid == userid).Select(m => m.datasetid).ToArray();
                    objDatasets = _dbContext.datasets.Where(ds => datasetIds.Contains(ds.id)).ToList();
                }

                return Ok(new
                {
                    success = 1,
                    data = objDatasets
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

        [NonAction]
        private bool isAuthorisedUser()
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == username);
                return userObj.admin;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
