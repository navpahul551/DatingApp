using System.Net;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {

        private readonly DataContext _context;

        public BuggyController(DataContext context) 
        {
            _context = context;
        }

        [HttpGet("auth")]
        [Authorize]
        public ActionResult<string> GetAuthError() 
        {
            return "";
        }

        [HttpGet("server-error")]
        public ActionResult<string> Get500Error() 
        {
            var user = _context.Users.Find(-1);
            var temp = user.ToString();
            return temp;    
        }
        
        [HttpGet("not-found")]
        public ActionResult GetNotFoundError()
        {
            return NotFound();
        }

        [HttpGet("bad-request")]
        public ActionResult GetBadRequest() 
        {
            return BadRequest("Invalid data!");
        }
    }
}