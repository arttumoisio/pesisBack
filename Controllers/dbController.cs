using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace pesisBackend.Controllers
{
    [ApiController]
    [Route("")] // [Route("testi")] -> route on /testi, nyt se on /
    public class dbController : ControllerBase
    {
        private readonly SQLiteQueries _query2;

        public dbController()
        {
            _query2 = new SQLiteQueries();

        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetTest(
          int kaudetAlku=2000,
          int kaudetLoppu=2020
        )
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            string data = _query2.apuJoukkueet(kaudetAlku,kaudetLoppu);
            if (data == ""){return StatusCode(404); }
            return Ok(data);
        }
        
        [HttpGet("pelaajat")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetPelaajat(
          int kaudetAlku=2000,
          int kaudetLoppu=2020, 
          Boolean vuosittain=false
        )
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            string data = "";
            if (vuosittain) { data = _query2.haePelaajatVuosittain(kaudetAlku,kaudetLoppu);} 
            else { data = _query2.haePelaajat(kaudetAlku,kaudetLoppu); }
            if (data == ""){return StatusCode(404); }
            return Ok(data);
        }

        [HttpGet("joukkueet")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetJoukkueet(
          int kaudetAlku=2000,
          int kaudetLoppu=2020, 
          Boolean vuosittain=false,
          string joukkue = "Mikä tahansa"
        )
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            string data = "";
            if (vuosittain) { data = _query2.haeJoukkueetVuosittain(kaudetAlku,kaudetLoppu,joukkue);} 
            else { data = _query2.haeJoukkueet(kaudetAlku,kaudetLoppu,joukkue); }
            if (data == ""){return StatusCode(404); }
            return Ok(data);
        }
    }
}
