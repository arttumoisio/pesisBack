using System;
using Newtonsoft.Json;

namespace pesisBackend
{
    interface ITuomariParams : IBasicParams
    {
        string kotijoukkue {get; set;}
        string vierasjoukkue {get; set;}
        string lukkari {get; set;}
        string STPT {get; set;}
    }
}