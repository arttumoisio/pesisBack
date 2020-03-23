using System;
using System.Diagnostics;
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
          Boolean vuosittain=false,
          string paikka="",
          string tulos="",
          string sarja="",
          string sarjavaihe="",
          string vastustaja="",
          string joukkue= ""

        )
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            string data = "";
            if (vuosittain) { data = _query2.haePelaajatVuosittain(kaudetAlku,kaudetLoppu,paikka,tulos,vastustaja,joukkue,sarja,sarjavaihe);} 
            else { data = _query2.haePelaajat(kaudetAlku,kaudetLoppu,paikka,tulos,vastustaja,joukkue,sarja,sarjavaihe); }
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
          string joukkue = "",
          string paikka="",
          string tulos="",
          string sarja="",
          string sarjavaihe="",
          string vastustaja=""
        )
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            string data = "";
            if (vuosittain) { data = _query2.haeJoukkueetVuosittain(kaudetAlku,kaudetLoppu,joukkue,paikka,tulos,vastustaja,sarja,sarjavaihe);} 
            else { data = _query2.haeJoukkueet(kaudetAlku,kaudetLoppu,joukkue,paikka,tulos,vastustaja,sarja,sarjavaihe); }
            if (data == ""){return StatusCode(404); }
            return Ok(data);
        }

        [HttpGet("tuomarit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetTuomarit(
          int kaudetAlku=2000,
          int kaudetLoppu=2020, 
          Boolean vuosittain=false,
          string kotijoukkue = "",
          string vierasjoukkue = "",
          string sarja="",
          string sarjavaihe="",
          string lukkari = "",
          string STPT = ""
        )
        {   
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine(Request.Query);
            Console.WriteLine("Request.QueryString");
            Console.WriteLine(Request.QueryString);
            Console.WriteLine("Request.QueryString");
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            string data = "";
            data = _query2.haeTuomarit(kaudetAlku,kaudetLoppu,vuosittain,kotijoukkue,vierasjoukkue, lukkari, STPT,sarja,sarjavaihe);
            sw.Stop();
            Console.WriteLine("TUOMAREIHIN AIKAA MENI: {0}",sw.Elapsed);
            if (data == ""){return StatusCode(404); }
            return Ok(data);
        }
    }
}
