using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace generic_repo_uow_pattern_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExceptionHandlerController : ControllerBase
    {
        [HttpGet("NoException")]
        public ActionResult NoException()
        {
            return Ok(".net realworld example");
        }

        [HttpGet("NoImplementException")]
        public ActionResult GetNoImplementationException()
        {
            throw new NotImplementedException();
        }

        [HttpGet("TimeoutException")]
        public ActionResult GetTimeoutException()
        {
            throw new TimeoutException();
        }
    }
}