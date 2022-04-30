using canonapi.Authentication;
using canonapi.configuration;
using canonapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace canonapi.Controllers
{
    public class SessionController : Controller
    {

        private ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IAnnotationService _annotationService;

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public SessionController(ApplicationDbContext context, IConfiguration configuration, IAnnotationService annotationService)
        {
            _configuration = configuration;
            _dbContext = context;
            _annotationService = annotationService;
        }

        public static bool createSession(ApplicationDbContext dbContext,int userid, string sessionkey)
        {
            try
            {
                Sessions objSession = new Sessions();
                objSession.userid = userid;
                objSession.sessionkey = sessionkey;
                objSession.logintime = DateTime.Now;
                objSession.lastactivitytime = DateTime.Now;

                dbContext.sessions.Add(objSession);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static bool updateSession(ApplicationDbContext dbContext,string sessionkey)
        {
            try
            {
                Sessions session = dbContext.sessions.Where(s=> s.sessionkey==sessionkey).FirstOrDefault();
                session.lastactivitytime = DateTime.Now;
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
