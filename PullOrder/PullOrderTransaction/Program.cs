using Microsoft.Extensions.Configuration;
using System.IO;
using PullOrder.Base.Models;
using System;
using System.Threading;

namespace PullOrderTransaction
{
    class Program
    {
        static void Main(string[] args)
        {

            PullOrdersSStart process = new PullOrdersSStart();
            process.Start();
        }
    }
}
