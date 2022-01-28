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
                    User objUserFromDB = null;
                    objUserFromDB = _dbContext.users.Where(d => d.username == objs.username).FirstOrDefault();
                    if (objUserFromDB != null)
                    {
                        objUserFromDB.userpassword = objs.userpassword;
                        objUserFromDB.firstname = objs.firstname;
                        objUserFromDB.lastname = objs.lastname;
                        objUserFromDB.admin = objs.admin;

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
                    User objUserFromDB = null;
                    objUserFromDB = _dbContext.users.Where(d => d.username == objs.username).FirstOrDefault();
                    if (objUserFromDB != null)
                    {
                        _dbContext.users.Remove(objUserFromDB);
                        _dbContext.datasetmap.RemoveRange(_dbContext.datasetmap.Where(dm => dm.userid == objUserFromDB.id));
                        IEnumerable<Datasets> lstDataset = _dbContext.datasets.Where(d => d.adminid == objUserFromDB.id.ToString());
                        foreach (Datasets dataset in lstDataset)
                        {
                            dataset.adminid = null;
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
                            message = "No matching dataset found"
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        success = 0,
                        message = "Invalid inputs"
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
                    DatasetMap objDsMapFromDB = null;
                    objDsMapFromDB = _dbContext.datasetmap.Where(ds => ds.userid == datasetMap.userid && ds.userid == datasetMap.datasetid).First();
                    if (objDsMapFromDB == null)
                    {
                        if (datasetMap.isadmin)
                        {
                            Datasets dsInDB = _dbContext.datasets.Where(d => d.id == datasetMap.id).FirstOrDefault();       //get dataset
                            if (dsInDB != null)
                            {
                                if (dsInDB.adminid != datasetMap.userid.ToString())                 //Check if adminid for the dataset is set to user in the current object
                                {
                                    dsInDB.adminid = datasetMap.userid.ToString();                  //Add adminid in database
                                }
                            }
                        }
                        _dbContext.datasetmap.Add(datasetMap);
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        if (objDsMapFromDB.isadmin && !datasetMap.isadmin)                          //admin in database but not in sent object
                        {
                            Datasets dsInDB = _dbContext.datasets.Where(d => d.id == datasetMap.id).FirstOrDefault();       //get dataset
                            if (dsInDB != null)
                            {
                                if (dsInDB.adminid == datasetMap.userid.ToString())                 //Check if adminid for the dataset is set to user in the current object
                                {
                                    dsInDB.adminid = null;                                          //Remove adminid from database
                                }
                            }
                        }
                        else if (!objDsMapFromDB.isadmin && datasetMap.isadmin)                     //admin in sent object but not in database
                        {
                            Datasets dsInDB = _dbContext.datasets.Where(d => d.id == datasetMap.id).FirstOrDefault();       //get dataset
                            if (dsInDB != null)
                            {
                                if (dsInDB.adminid != datasetMap.userid.ToString())                 //Check if adminid for the dataset is set to user in the current object
                                {
                                    dsInDB.adminid = datasetMap.userid.ToString();                  //Add adminid in database
                                }
                            }
                        }
                        objDsMapFromDB.isadmin = datasetMap.isadmin;
                        objDsMapFromDB.isanonymous = datasetMap.isanonymous;
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
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var currentUsername = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == currentUsername);

                //List<DatasetsForUser> lstOut = new List<DatasetsForUser>();

                DatasetsForUser lstout;

                if (String.IsNullOrEmpty(username))
                {
                    return Ok(new
                    {
                        success = default(int),
                        message = "Username cannot be empty"
                    });
                }
                else
                {
                    if (userObj.admin || username == currentUsername)
                    {
                        lstout = new DatasetsForUser();
                        lstout.user = _dbContext.users.SingleOrDefault(u => u.username == username);
                        if (lstout.user != null)
                        {
                            lstout.lstMappedDatasets = (from dm in _dbContext.datasetmap
                                                                 join d in _dbContext.datasets on dm.datasetid equals d.id
                                                                 where dm.userid == lstout.user.id
                                                                 orderby dm.id
                                                                 select new MappedDatasets()
                                                                 {
                                                                     datasetid = d.id,
                                                                     datasetname = d.datasetname,
                                                                     description = d.description,
                                                                     attribute = d.attribute,
                                                                     referenceid = d.referenceid,
                                                                     adminid = d.adminid,
                                                                     isadmin = dm.isadmin,
                                                                     isanonymous = dm.isanonymous
                                                                 }).ToList();
                        }
                        foreach(MappedDatasets mappedDataset in lstout.lstMappedDatasets)
                        {
                            mappedDataset.totalimages = _dbContext.images.Where(i => i.datasetid == mappedDataset.datasetid).Count();
                        }
                        //lstOut.Add(datasetsForUser);
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = default(int),
                            message = "User does not have enough rights"
                        });
                    }
                }
                #region Nested Structure

                ////if (String.IsNullOrEmpty(username))
                ////{
                ////    if (userObj.admin)
                ////    {
                ////        foreach (User user in _dbContext.users.ToList())
                ////        {
                ////            DatasetsForUser datasetsForUser = new DatasetsForUser();
                ////            datasetsForUser.user = user;
                ////            datasetsForUser.lstMappedDatasets = (from dm in _dbContext.datasetmap
                ////                                                 join d in _dbContext.datasets on dm.datasetid equals d.id
                ////                                                 where dm.userid == datasetsForUser.user.id
                ////                                                 orderby dm.id
                ////                                                 select new MappedDatasets()
                ////                                                 {
                ////                                                     datasetid = d.id,
                ////                                                     datasetname = d.datasetname,
                ////                                                     description = d.description,
                ////                                                     attribute = d.attribute,
                ////                                                     referenceid = d.referenceid,
                ////                                                     adminid = d.adminid,
                ////                                                     isadmin = dm.isadmin,
                ////                                                     isanonymous = dm.isanonymous
                ////                                                 }).ToList();
                ////            lstOut.Add(datasetsForUser);
                ////        }
                ////    }
                ////    else
                ////    {
                ////        DatasetsForUser datasetsForUser = new DatasetsForUser();
                ////        datasetsForUser.user = userObj;
                ////        datasetsForUser.lstMappedDatasets = (from dm in _dbContext.datasetmap
                ////                                             join d in _dbContext.datasets on dm.datasetid equals d.id
                ////                                             where dm.userid == datasetsForUser.user.id
                ////                                             orderby dm.id
                ////                                             select new MappedDatasets()
                ////                                             {
                ////                                                 datasetid = d.id,
                ////                                                 datasetname = d.datasetname,
                ////                                                 description = d.description,
                ////                                                 attribute = d.attribute,
                ////                                                 referenceid = d.referenceid,
                ////                                                 adminid = d.adminid,
                ////                                                 isadmin = dm.isadmin,
                ////                                                 isanonymous = dm.isanonymous
                ////                                             }).ToList();
                ////        lstOut.Add(datasetsForUser);
                ////    }
                ////}
                ////else
                ////{
                ////    if (userObj.admin || username == currentUsername)
                ////    {
                ////        DatasetsForUser datasetsForUser = new DatasetsForUser();
                ////        datasetsForUser.user = _dbContext.users.SingleOrDefault(u => u.username == username);
                ////        if (datasetsForUser.user != null)
                ////        {
                ////            datasetsForUser.lstMappedDatasets = (from dm in _dbContext.datasetmap
                ////                                                 join d in _dbContext.datasets on dm.datasetid equals d.id
                ////                                                 where dm.userid == datasetsForUser.user.id
                ////                                                 orderby dm.id
                ////                                                 select new MappedDatasets()
                ////                                                 {
                ////                                                     datasetid = d.id,
                ////                                                     datasetname = d.datasetname,
                ////                                                     description = d.description,
                ////                                                     attribute = d.attribute,
                ////                                                     referenceid = d.referenceid,
                ////                                                     adminid = d.adminid,
                ////                                                     isadmin = dm.isadmin,
                ////                                                     isanonymous = dm.isanonymous
                ////                                                 }).ToList();
                ////        }
                ////        lstOut.Add(datasetsForUser);
                ////    }
                ////    else
                ////    {
                ////        return Ok(new
                ////        {
                ////            success = default(int),
                ////            message = "User does not have enough rights"
                ////        });
                ////    }
                ////}

                #endregion
                return Ok(new
                {
                    success = 1,
                    data = lstout
                });

                #region Commented
                #region using linq function
                //var result = _dbContext.users.Join(_dbContext.datasetmap, u => u.id, dm => dm.userid, (u, dm) => new
                //{
                //    u.username,
                //    u.userpassword,
                //    u.firstname,
                //    u.lastname,
                //    adminuser = u.admin,
                //    datasetmapid = dm.id,
                //    dm.userid,
                //    dm.datasetid,
                //    dm.isadmin,
                //    dm.isanonymous
                //})
                //            .Join(_dbContext.datasets, v => v.datasetid, d => d.id, (v, d) => new
                //            {
                //                v.datasetmapid,
                //                v.userid,
                //                v.username,
                //                v.userpassword,
                //                v.firstname,
                //                v.lastname,
                //                v.adminuser,
                //                v.datasetid,
                //                d.datasetname,
                //                datasetdescription = d.description,
                //                d.attribute,
                //                d.adminid,
                //                d.referenceid,
                //                v.isanonymous
                //            }).ToList(); 
                #endregion

                //var resultQuery = (from u in _dbContext.users
                //                   join dm in _dbContext.datasetmap on u.id equals dm.userid
                //                   join d in _dbContext.datasets on dm.datasetid equals d.id
                //                   select new
                //                   {
                //                       datasetmapid = dm.id,
                //                       dm.userid,
                //                       u.username,
                //                       //u.userpassword,
                //                       //u.firstname,
                //                       //u.lastname,
                //                       adminuser = u.admin,
                //                       dm.datasetid,
                //                       d.datasetname,
                //                       datasetdescription = d.description,
                //                       d.attribute,
                //                       datasetadminid = d.adminid,
                //                       d.referenceid,
                //                       dm.isadmin,
                //                       dm.isanonymous,
                //                   }).ToList();

                //if (String.IsNullOrEmpty(username))
                //{
                //    return Ok(new
                //    {
                //        success = 1,
                //        data = userObj.admin ? resultQuery : resultQuery.Where(r => r.username == currentUsername).ToList()
                //    });
                //}
                //else
                //{
                //    if (userObj.admin || username == currentUsername)
                //    {
                //        return Ok(new
                //        {
                //            success = 1,
                //            data = resultQuery.Where(r => r.username == username.Trim()).ToList()
                //        });
                //    }
                //    else
                //    {
                //        return Ok(new
                //        {
                //            success = default(int),
                //            message = "User does not have enough rights"
                //        });
                //    }
                //} 
                #endregion
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
        [ActionName("GetUserDetails")]
        public IActionResult GetUserDetails([FromQuery] string username = null)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var currentUsername = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                User userObj = _dbContext.users.SingleOrDefault(u => u.username == currentUsername);

                if (String.IsNullOrEmpty(username))
                {
                    return Ok(new
                    {
                        success = 1,
                        data = userObj.admin ? _dbContext.users.ToList() : _dbContext.users.Where(u => u.username == currentUsername).ToList()
                    });
                }
                else
                {
                    if (userObj.admin || username == currentUsername)
                    {
                        return Ok(new
                        {
                            success = 1,
                            data = _dbContext.users.Where(u => u.username == username.Trim()).ToList()
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = default(int),
                            message = "User does not have enough rights"
                        });
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
