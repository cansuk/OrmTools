using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePlus.BusinessContracts;
using BasePlus.Common;
using BasePlus.Common.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BasePlus.Controllers
{
    //[Route("api/[controller]/[action]")]
    [ApiController]
    public class OrmController : ControllerBase
    {
        readonly IOrmService _ormService;
        public OrmController(IOrmService ormService)
        {
            _ormService = ormService;
        }

        [HttpGet]
        [ActionName("TestAllOrms")]
        [Route("api/[controller]/[action]/{testCount}")]
        //  public List<TestResult> TestAllOrms(int testCount)
        public List<TestResult> TestAllOrms(int testCount)
        {
            List<TestResult> results = new List<TestResult>();

            for (int i = 0; i < testCount; i++)
            {
                results.AddRange(_ormService.TestOrms());
            }
         
            return results;
        }

        [HttpGet]
        [ActionName("GetScoreAnlyzes")]
        [Route("api/[controller]/[action]")]
        public List<Score> GetScoreAnlyzes()
        {
            // int testCount = 5;
            var result = new List<Score>();
            try
            {
                result = _ormService.GetAnalyzes(); ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            return result;
        }


    }
}