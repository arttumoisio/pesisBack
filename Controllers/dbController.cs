using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace pesisBackend.Controllers
{
    [ApiController]
    [Route("")] // [Route("testi")] -> route on /testi, nyt se on /
    public class dbController : ControllerBase
    {
        private readonly SQLiteQueries query;
        private IQueryTimer queryTimer;
        private readonly ReturnStatusHandler returnStatusHandler;

        public dbController()
        {
            query = new SQLiteQueries();
            queryTimer = new QueryTimer();
            returnStatusHandler = new ReturnStatusHandler();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetTest(
        )
        {
            return Ok("Serveri toimii");
        }
        
        [HttpGet("pelaajat")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetPelaajat(
          int kaudetAlku=2000,
          int kaudetLoppu=2020, 
          string sarja="Miesten superpesis",
          string sarjavaihe="Runkosarja",
          Boolean vuosittain=false,
          string paikka="",
          string tulos="",
          string vastustaja="",
          string joukkue = ""
        )
        {
            ITeamParams teamParams = new TeamParams(
                kaudetAlku,
                kaudetLoppu, 
                sarja,
                sarjavaihe,
                vuosittain,
                paikka,
                tulos,
                vastustaja,
                joukkue 
            );
            
            queryTimer.Start();
            
            string data = query.haePelaajat(teamParams);
            
            queryTimer.Stop("PELAAJIIN");
            
            return returnStatusHandler.handleResultString(data);
        }

        [HttpGet("joukkueet")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetJoukkueet(
          int kaudetAlku=2000,
          int kaudetLoppu=2020, 
          string sarja="Miesten superpesis",
          string sarjavaihe="Runkosarja",
          Boolean vuosittain=false,
          string paikka="",
          string tulos="",
          string vastustaja="",
          string joukkue = ""
        )
        {
            ITeamParams teamParams = new TeamParams(
                kaudetAlku,
                kaudetLoppu, 
                sarja,
                sarjavaihe,
                vuosittain,
                paikka,
                tulos,
                vastustaja,
                joukkue 
            );


            queryTimer.Start();

            string data = query.haeJoukkueet(teamParams);

            
            queryTimer.Stop("JOUKKUEISIIN");
            
            return returnStatusHandler.handleResultString(data);
        }

        [HttpGet("tuomarit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetTuomarit(
          int kaudetAlku=2000,
          int kaudetLoppu=2020, 
          string sarja="",
          string sarjavaihe="",
          Boolean vuosittain=false,
          string kotijoukkue = "",
          string vierasjoukkue = "",
          string lukkari = "",
          string STPT = ""
        )
        {   
            ITuomariParams tuomariParams = new TuomariParams(
                kaudetAlku,
                kaudetLoppu, 
                sarja,
                sarjavaihe,
                vuosittain=false,
                kotijoukkue,
                vierasjoukkue,
                lukkari,
                STPT
            );
            
            queryTimer.Start();
            
            string data = query.haeTuomarit(tuomariParams);

            queryTimer.Stop("TUOMAREIHIN");
            
            return returnStatusHandler.handleResultString(data);
        }
    }
}
