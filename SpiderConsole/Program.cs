using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpiderJobs;

namespace SpiderConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Start1010JobsSpider spider1010 = new Start1010JobsSpider();
            spider1010.Run();
        }
    }
}
