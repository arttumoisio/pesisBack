using System;
using Newtonsoft.Json;

namespace pesisBackend
{
    interface ITeamParams : IBasicParams
    {
        string koti {get; set;}
        string tulos {get; set;}
        string vastustaja {get; set;}
        string joukkue {get; set;}
    }
}