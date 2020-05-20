using System;
using Newtonsoft.Json;

namespace pesisBackend
{
    interface IBasicParams
    {
        int kaudetAlku {get; set;}
        int kaudetLoppu {get; set;}
        string sarja {get; set;}
        string sarjajako {get; set;}
        bool vuosittain {get; set;}
    }
}