using System;
using System.IO;
using System.Data.SQLite;
using System.Data;
using Newtonsoft.Json;

namespace pesisBackend
{
    interface IDBHandler
    {
        string ajaKyselyParametreilla(string query, IBasicParams basicParams);
        string ajaKyselyParametreilla(string query, ITuomariParams tuomariParams);
        string ajaKyselyParametreilla(string query, ITeamParams teamParams);

        string ajaKysely(string query);
    }
}