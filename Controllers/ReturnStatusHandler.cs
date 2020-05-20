using System;
using Microsoft.AspNetCore.Mvc;

namespace pesisBackend.Controllers
{
    [ApiController]
    class ReturnStatusHandler : ControllerBase
    {
        public IActionResult handleResultString(string queryResult){
            
            if (queryResult == ""){
                return StatusCode(404); 
            }
            return Ok(queryResult);
        }
    }
}