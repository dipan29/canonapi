using canonapi.Authentication;
using canonapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace canonapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DiseaseController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public DiseaseController(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbContext = context;
        }

        [HttpGet]
        [ActionName("GetDisease")]
        public IActionResult GetDisease()
        {
            try
            {
                List<Disease> result = new List<Disease>();
                result = _dbContext.ExecuteQuery<Disease>(QueryHelper.qryDiseaseMap);
                if (result != null && result.Count() > default(int))
                {
                    result.Add(new Disease()
                    {
                        diseaseid = DRStatus.DR0.GetHashCode(),
                        diseasename = "DR0",
                        subdiseaseid = default(int),
                        subdieseasename = string.Empty
                    });
                }
                return Ok(new
                {
                    success = 1,
                    data = result.OrderBy(d => d.diseaseid)
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
    }
}
