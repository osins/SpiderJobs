using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderZYM
{
    class Program
    {
        static void Main(string[] args)
        {
            SpiderTaobao taobao = new SpiderTaobao();
            taobao.Start();
        }
    }
}
