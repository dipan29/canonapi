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
                    int adminid = -1;
                    if (String.IsNullOrEmpty(objSource.adminid) || !Int32.TryParse(objSource.adminid, out adminid))
                    {
                        objSource.adminid = objUser.id.ToString();
                    }
                    _dbContext.datasets.Add(objSource);
                    _dbContext.SaveChanges();

                    DatasetMap objDatasetMap = new DatasetMap()
                    {
                        id = 0,
                        userid = adminid > 0 ? adminid : objUser.id,
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
        public IActionResult UpdateDataset([FromBody] Datasets objInput)
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
                if (objInput != null)
                {
                    Datasets objdsFromDB = null;
                    objdsFromDB = _dbContext.datasets.Where(d => d.datasetname == objInput.datasetname).FirstOrDefault();
                    if (objdsFromDB != null)
                    {
                        objdsFromDB.description = objInput.description;
                        objdsFromDB.attribute = objInput.attribute;
                        objdsFromDB.referenceid = objInput.referenceid;
                        if (objdsFromDB.adminid != objInput.adminid)                    //chaging admin user id for the dataset
                        {
                            //Step 1: remove isadmin from datasetmap for the existing user
                            DatasetMap datasetMap1 = null;
                            if (objdsFromDB.adminid != null)                            //if admin user was previously mentioned at all(hence not null check)
                            {
                                datasetMap1 = _dbContext.datasetmap.Where(x => x.datasetid == objdsFromDB.id && x.userid.ToString() == objdsFromDB.adminid).FirstOrDefault();
                                if (datasetMap1 != null)
                                {
                                    if (datasetMap1.isadmin)
                                    {
                                        datasetMap1.isadmin = false;
                                    }
                                }
                            }
                            //Step 2: add isadmin in datasetmap for new user
                            DatasetMap datasetMap2 = null;
                            if (objInput.adminid != null)                            //if any admin user is mentioned in the current object(hence not null check)
                            {
                                datasetMap2 = _dbContext.datasetmap.Where(x => x.datasetid == objdsFromDB.id && x.userid.ToString() == objInput.adminid).FirstOrDefault();
                                if (datasetMap2 != null)
                                {
                                    if (!datasetMap2.isadmin)
                                    {
                                        datasetMap2.isadmin = true;
                                    }
                                }
                            }
                            objdsFromDB.adminid = objInput.adminid;
                        }
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
                    Datasets dsFromDB = _dbContext.datasets.Where(d => d.datasetname == objs.datasetname).FirstOrDefault();
                    if (dsFromDB != null)
                    {
                        _dbContext.datasets.Remove(dsFromDB);
                        _dbContext.datasetmap.RemoveRange(_dbContext.datasetmap.Where(x => x.datasetid == dsFromDB.id));
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
                else
                {
                    return Ok(new
                    {
                        success = 0,
                        message = "Invalid Input"
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

        [HttpGet]
        [ActionName("GetDatasetDetails")]
        public IActionResult GetDatasetDetails(string datasetname = null)
        {
            try
            {
                User objLeggedInUser = GetUser();

                #region checking input string first commented
                //if (String.IsNullOrEmpty(datasetname))
                //{
                //    if (objUser.admin)
                //    {
                //        return Ok(new
                //        {
                //            success = 1,
                //            data = _dbContext.datasets.ToList()
                //        });
                //    }
                //    else
                //    {
                //        int[] allowedDatasets = _dbContext.datasetmap.Where(dm => dm.userid == objUser.id).Select(dm => dm.datasetid).ToArray();
                //        return Ok(new
                //        {
                //            success = 1,
                //            data = _dbContext.datasets.Where(d => allowedDatasets.Contains(d.id)).ToList()
                //        });
                //    }
                //}
                //else
                //{
                //    if (objUser.admin)
                //    {
                //        return Ok(new
                //        {
                //            success = 1,
                //            data = _dbContext.datasets.Where(d => d.datasetname == datasetname).ToList()
                //        });
                //    }
                //    else
                //    {
                //        int[] allowedDatasets = _dbContext.datasetmap.Where(dm => dm.userid == objUser.id).Select(dm => dm.datasetid).ToArray();
                //        Datasets outds = _dbContext.datasets.Where(d => d.datasetname == datasetname).FirstOrDefault();
                //        if (allowedDatasets.Contains(outds.id))
                //        {
                //            return Ok(new
                //            {
                //                success = 1,
                //                data = outds
                //            });
                //        }
                //        else
                //        {
                //            return Ok(new
                //            {
                //                success = default(int),
                //                message = "User does not have access to the given dataset"
                //            });
                //        }
                //    }
                //} 
                #endregion

                if (objLeggedInUser.admin)
                {
                    return Ok(new
                    {
                        success = 1,
                        data = String.IsNullOrEmpty(datasetname) ? _dbContext.datasets.ToList() : _dbContext.datasets.Where(d => d.datasetname == datasetname).ToList()
                    });
                }
                else
                {
                    int[] allowedDatasets = _dbContext.datasetmap.Where(dm => dm.userid == objLeggedInUser.id).Select(dm => dm.datasetid).ToArray();
                    if (String.IsNullOrEmpty(datasetname))
                    {
                        return Ok(new
                        {
                            success = 1,
                            data = _dbContext.datasets.Where(d => allowedDatasets.Contains(d.id)).ToList()
                        });
                    }
                    else
                    {
                        Datasets outds = _dbContext.datasets.Where(d => d.datasetname == datasetname).FirstOrDefault();
                        if (allowedDatasets.Contains(outds.id))
                        {
                            return Ok(new
                            {
                                success = 1,
                                data = outds
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                success = default(int),
                                message = "User does not have access to the given dataset"
                            });
                        }
                    }
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

        [HttpGet]
        [ActionName("GetUsersForDatset")]
        public IActionResult GetUsersForDatset([FromQuery] string datasetname = null)
        {
            try
            {
                User loggedInUser = GetUser();

                List<UsersForDataset> lstOut = new List<UsersForDataset>();

                if (String.IsNullOrEmpty(datasetname))
                {
                    if (loggedInUser.admin)
                    {
                        foreach (Datasets dataset in _dbContext.datasets.ToList())
                        {
                            UsersForDataset usersForDataset = new UsersForDataset();
                            usersForDataset.dataset = dataset;
                            usersForDataset.lstMappedUsers = (from dm in _dbContext.datasetmap
                                                              join u in _dbContext.users on dm.userid equals u.id
                                                              where dm.datasetid == dataset.id
                                                              orderby dm.id
                                                              select new MappedUsers()
                                                              {
                                                                  userid = u.id,
                                                                  username = u.username,
                                                                  firstname = u.firstname,
                                                                  lastname = u.lastname,
                                                                  adminuser = u.admin,
                                                                  isadmin = dm.isadmin,
                                                                  isanonymous = dm.isanonymous
                                                              }).ToList();
                            lstOut.Add(usersForDataset);
                        }
                    }
                    else
                    {
                        int[] allowedDatasets = _dbContext.datasetmap.Where(dm => dm.userid == loggedInUser.id).Select(dm => dm.datasetid).ToArray();
                        foreach (Datasets dataset in _dbContext.datasets.Where(d => allowedDatasets.Contains(d.id)).ToList())
                        {
                            UsersForDataset usersForDataset = new UsersForDataset();
                            usersForDataset.dataset = dataset;
                            usersForDataset.lstMappedUsers = (from dm in _dbContext.datasetmap
                                                              join u in _dbContext.users on dm.userid equals u.id
                                                              orderby dm.id
                                                              where dm.datasetid == dataset.id
                                                              select new MappedUsers()
                                                              {
                                                                  userid = u.id,
                                                                  username = u.username,
                                                                  firstname = u.firstname,
                                                                  lastname = u.lastname,
                                                                  adminuser = u.admin,
                                                                  isadmin = dm.isadmin,
                                                                  isanonymous = dm.isanonymous
                                                              }).ToList();

                            lstOut.Add(usersForDataset);
                        }
                    }
                }
                else
                {
                    if (loggedInUser.admin)
                    {
                        UsersForDataset usersForDataset = new UsersForDataset();
                        usersForDataset.dataset = _dbContext.datasets.Where(d => d.datasetname == datasetname).FirstOrDefault();
                        if (usersForDataset.dataset != null)
                        {
                            usersForDataset.lstMappedUsers = (from dm in _dbContext.datasetmap
                                                              join u in _dbContext.users on dm.userid equals u.id
                                                              where dm.datasetid == usersForDataset.dataset.id
                                                              orderby dm.id
                                                              select new MappedUsers()
                                                              {
                                                                  userid = u.id,
                                                                  username = u.username,
                                                                  firstname = u.firstname,
                                                                  lastname = u.lastname,
                                                                  adminuser = u.admin,
                                                                  isadmin = dm.isadmin,
                                                                  isanonymous = dm.isanonymous
                                                              }).ToList();
                        }
                        lstOut.Add(usersForDataset);
                    }
                    else
                    {
                        int[] allowedDatasets = _dbContext.datasetmap.Where(dm => dm.userid == loggedInUser.id).Select(dm => dm.datasetid).ToArray();
                        Datasets ds = _dbContext.datasets.Where(d => d.datasetname == datasetname).FirstOrDefault();
                        if(allowedDatasets.Contains(ds.id))
                        {
                            UsersForDataset usersForDataset = new UsersForDataset();
                            usersForDataset.dataset = ds;
                            if (usersForDataset.dataset != null)
                            {
                                usersForDataset.lstMappedUsers = (from dm in _dbContext.datasetmap
                                                                  join u in _dbContext.users on dm.userid equals u.id
                                                                  where dm.datasetid == usersForDataset.dataset.id
                                                                  orderby dm.id
                                                                  select new MappedUsers()
                                                                  {
                                                                      userid = u.id,
                                                                      username = u.username,
                                                                      firstname = u.firstname,
                                                                      lastname = u.lastname,
                                                                      adminuser = u.admin,
                                                                      isadmin = dm.isadmin,
                                                                      isanonymous = dm.isanonymous
                                                                  }).ToList();
                            }
                            lstOut.Add(usersForDataset);
                        }
                        else
                        {
                            return Ok(new
                            {
                                success = default(int),
                                message = "User does not have access to the given dataset"
                            });
                        }
                    }
                }
                return Ok(new
                {
                    success = 1,
                    data = lstOut
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
