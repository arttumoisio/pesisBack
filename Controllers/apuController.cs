using Microsoft.AspNetCore.Mvc;
using System;

namespace pesisBackend.Controllers
{
    [ApiController]
    [Route("apu")] // [Route("testi")] -> route on /testi, nyt se on /
    public class apuController : ControllerBase
    {
        private readonly SQLiteQueries _query2;

        public apuController()
        {
            _query2 = new SQLiteQueries();

        }

        [HttpGet("joukkueet")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetJoukkueet(
          int kaudetAlku=2000,
          int kaudetLoppu=2019
        )
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            Console.WriteLine(kaudetAlku.ToString(),kaudetLoppu.ToString());
            string data = _query2.apuJoukkueet(kaudetAlku,kaudetLoppu);
            if (data == ""){return StatusCode(404); }
            return Ok(data);
        }
        [HttpGet("vuodet")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetVuodet(
        )
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            string data = _query2.apuVuodet();
            if (data == ""){return StatusCode(404); }
            return Ok(data);
        }
        [HttpGet("sarjavaihe")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetVaiheet(
        )
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            string data = _query2.apuSarjaVaiheet();
            if (data == ""){return StatusCode(404); }
            return Ok(data);
        }
        [HttpGet("lyontinumero")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetLyontiNumerot(
        )
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            string data = _query2.apuLyontiNumerot();
            if (data == ""){return StatusCode(404); }
            return Ok(data);
        }
        [HttpGet("ulkopelipaikka")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetUPPaikat(
        )
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            string data = _query2.apuUlkoPeliPaikat();
            if (data == ""){return StatusCode(404); }
            return Ok(data);
        }
        [HttpGet("lukkarit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetLukkarit(
            int kaudetAlku = 2000,
            int kaudetLoppu = 2020
        )
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            string data = _query2.apuLukkarit(kaudetAlku,kaudetLoppu);
            if (data == ""){return StatusCode(404); }
            return Ok(data);
        }
    }
}
