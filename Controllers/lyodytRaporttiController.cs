using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace pesisBackend.Controllers
{
    [ApiController]
    [Route("[controller]")] // [Route("testi")] -> route on /testi, nyt se on lyodyt
    public class lyodytController : ControllerBase
    {
        private readonly ILogger<lyodytController> _logger;

        public lyodytController(ILogger<lyodytController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<LyodytLuokka> Get()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            return Enumerable.Range(1, 10).Select(index => new LyodytLuokka
            {
                pelaaja = "Kalle Moisio",
                lyodyt = 30*index,
                kunnarit = 2*index,
                ottelut = 10*index,
                yritykset = 39*index
            })
            .ToArray();
        }

        [HttpGet("lyhyt/{loppu}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
         public IActionResult GetById(int loppu)
        {
            Console.WriteLine(loppu);
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            if (loppu < 1)
            {
                return NotFound();
            }
            int alku = ((loppu-1) / 100) * 100 + 1;
            int montako = loppu % 100;
            var rng = new Random();
            var data = Enumerable.Range(alku, montako).Select(index => new Lyhyt
            {
                pelaaja = "Kalle Moisio" + (1000-index),
                lyodyt = rng.Next(30, 400),
                kunnarit = rng.Next(1, 29)
            }).ToArray().OrderByDescending(s => s.yht);

            return Ok(data);
        }
        

        [HttpGet("{loppu}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetByI(int loppu)
        {
            Console.WriteLine(loppu);
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            if (loppu < 1)
            {
                return NotFound();
            }
            int alku = ((loppu-1) / 100) * 100 + 1;
            int montako = loppu % 100;
            var rng = new Random();
            var data = Enumerable.Range(alku, montako).Select(index => new LyodytLuokka
            {
                pelaaja = "Kalle Moisio" + (1000-index),
                lyodyt = 30*index,
                kunnarit = 1*index,
                ottelut = 10*index,
                yritykset = rng.Next(alku*100, loppu*100)
            }).ToArray();

            return Ok(data);
        }

        [HttpGet("kaikki")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetKaikki()
        {
            int alku = 1;
            int loppu  = 9643;
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            if (loppu < 1)
            {
                return NotFound();
            }

            var rng = new Random();

            var data = Enumerable.Range(alku, loppu).Select(index => new Lyhyt
            {
                pelaaja = "Kalle Moisio" + (1000-index+1),
                lyodyt = rng.Next(30, 7000),
                kunnarit = rng.Next(1, 699)
            }).ToArray().OrderByDescending(s => s.yht);

            return Ok(data);
        }
    }
    
}


