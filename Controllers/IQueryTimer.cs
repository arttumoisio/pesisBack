using System;

namespace pesisBackend
{
    interface IQueryTimer
    {
        void Start();
        void Stop(string queryType);
    }
}