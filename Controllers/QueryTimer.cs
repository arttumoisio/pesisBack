using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace pesisBackend
{
    class QueryTimer : IQueryTimer
    {
        private Stopwatch stopWatch;
        public void Start()
        {  
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }

        public void Stop(string queryType){
            stopWatch.Stop();
            Console.WriteLine($"{queryType} AIKAA MENI: {stopWatch.Elapsed}");
        }
    }
}