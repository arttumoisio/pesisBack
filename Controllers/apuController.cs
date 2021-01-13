using Microsoft.AspNetCore.Mvc;
using System;

namespace pesisBackend.Controllers
{
    [ApiController]
    [Route("apu")]
    public class apuController : ControllerBase
    {
        private readonly ApuQueries query;
        private readonly ReturnStatusHandler returnStatusHandler;

        public apuController()
        {
            query = new DBFactory().GetApuQueries();
            returnStatusHandler = new ReturnStatusHandler();

        }

        [HttpGet("joukkueet")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetJoukkueet(
            int kaudetAlku=1994,
            int kaudetLoppu=2019,
            string sarja = "",
            string sarjavaihe = ""
        )
        {
            IBasicParams basicParams = new BasicParams(kaudetAlku,kaudetLoppu,sarja,sarjavaihe);

            string data = query.apuJoukkueet(basicParams);

            return returnStatusHandler.handleResultString(data);
        }
        [HttpGet("vuodet")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetVuodet(
        )
        {
            string data = query.apuVuodet();
           
            return returnStatusHandler.handleResultString(data);
        }
        [HttpGet("sarjavaihe")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetVaiheet(
          int kaudetAlku=1994,
          int kaudetLoppu=2019
        )
        {

            IBasicParams basicParams = new BasicParams(kaudetAlku,kaudetLoppu);

            string data = query.apusarjajaot(basicParams);
            
            return returnStatusHandler.handleResultString(data);
        }
        [HttpGet("sarja")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetSarjat(
          int kaudetAlku=1994,
          int kaudetLoppu=2019
        )
        {
            IBasicParams basicParams = new BasicParams(kaudetAlku,kaudetLoppu);

            string data = query.apuSarjat(basicParams);
            
            return returnStatusHandler.handleResultString(data);
        }
        
        [HttpGet("lukkarit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetLukkarit(
            int kaudetAlku = 1994,
            int kaudetLoppu = 2020,
            string sarja = "",
            string sarjavaihe = ""
        )
        {
            IBasicParams basicParams = new BasicParams(kaudetAlku,kaudetLoppu,sarja,sarjavaihe);

            Console.WriteLine(Request.Body);
            Console.WriteLine(Request.Query);
            Console.WriteLine(Request.QueryString);
            string data = query.apuLukkarit(basicParams);
            
            return returnStatusHandler.handleResultString(data);
        }
    }
}
