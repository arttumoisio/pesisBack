using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace pesisBackend
{
    interface IReturnStatusHandler
    {
        IActionResult handleResultString(string queryResult);
    }
}