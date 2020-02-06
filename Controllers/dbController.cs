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
    public class dbController : ControllerBase
    {
        private readonly ILogger<dbController> _logger;

        public dbController(ILogger<dbController> logger)
        {
            _logger = logger;
        }

        [HttpGet("sarake")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult Get()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            

            Console.WriteLine(Request.Host);
            var a = new SqliteService();
            var connection = a.connectorF();
            connection.Open();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
            SELECT p.nimi Nimi, sum(ku) Kunnarit, sum(ly) Lyödyt, sum(ku+ly) Yhteensä 
            FROM ottelu_tilasto o, pelaaja p 
            WHERE o.pelaaja_id = p.pelaaja_id 
            GROUP BY o.pelaaja_id 
            ORDER BY 3 DESC
            LIMIT 100;
            ";


            //IDictionary<String,String> data = new Dictionary<String, String>();
            IDictionary<String,List<String>> data = new Dictionary<String,List<String>>();
            //IList<String> data = new List<String>();

            
            using (var dr = selectCmd.ExecuteReader())
            {
              var r = 0;
              for (var ii = 0; ii < dr.FieldCount; ii++)
                {
                  Console.WriteLine(ii);
                  data.Add(dr.GetName(ii),new List<String>());//,dr.GetString(r))
                }
              while (dr.Read())
              {
                for (var i = 0; i < dr.FieldCount; i++)
                {
                  Console.WriteLine(i);
                  Console.WriteLine(r);
                  Console.WriteLine(dr.GetName(i));
                  Console.WriteLine(dr.GetString(i));

                  data[dr.GetName(i)].Add(dr.GetString(i));//,dr.GetString(r))
                }
                ++r;
              }
            }

            return Ok(data);
            
            //Enumerable.Range(1, 10).Select(index => new Object() );
            
                // pelaaja = "Kalle Moisio",
                // lyodyt = 30*index,
                // kunnarit = 2*index,
                // ottelut = 10*index,
                // yritykset = 39*index
            // })
            // .ToArray();

            

        }

        [HttpGet("rivi")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetRivi()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)Request.Headers["Origin"] });
            

            Console.WriteLine(Request.Host);
            var a = new SqliteService();
            var connection = a.connectorF();
            connection.Open();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
            SELECT p.nimi Nimi, sum(ku) Kunnarit, sum(ly) Lyödyt, sum(ku+ly) Yhteensä 
            FROM ottelu_tilasto o, pelaaja p 
            WHERE o.pelaaja_id = p.pelaaja_id 
            GROUP BY o.pelaaja_id 
            ORDER BY 3 DESC
            LIMIT 100000;
            ";


            //IDictionary<String,String> data = new Dictionary<String, String>();
            //IDictionary<String,List<String>> data = new Dictionary<String,List<String>>();
            //IList<String> data = new List<String>();
            IList<List<String>> data = new List<List<String>>();

            using (var dr = selectCmd.ExecuteReader())
            {
              var rivi = new List<String>();
              for (var i = 0; i < dr.FieldCount; i++)
                {
                  rivi.Add(dr.GetName(i));
                }
              data.Add(rivi);

              while (dr.Read())
              {
                rivi = new List<String>();
                for (var i = 0; i < dr.FieldCount; i++)
                {

                 rivi.Add(dr.GetString(i));
                }
                data.Add(rivi);
              }
            }
            return Ok(data);
        }
    }
}
