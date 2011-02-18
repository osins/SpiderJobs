using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Http;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Net;

namespace SpiderJobs
{
    public static class ParserHelp
    {
        public static Parser GetParser(string url)
        {
            ParserConf.GetConfiguration().RootPath = AppDomain.CurrentDomain.BaseDirectory;
            return new Parser(new HttpProtocol(new Uri(url)));
        }

        [DllImport("D:\\codes\\Spider\\AspriseOCR.dll", EntryPoint = "OCR")]
        public static extern IntPtr OCR(string file, int type);

        public static string GetImageCode(string _imgPath)
        {
            return Marshal.PtrToStringAnsi(OCR(_imgPath,-1));
        }
    }
}
