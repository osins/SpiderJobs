using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Http;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Filters;
using Winista.Text.HtmlParser.Tags;
using Winista.Text.HtmlParser.Visitors;
using Winista.Text.HtmlParser.Lex;
using System.Net;
using System.IO;
using Winista.Text.HtmlParser.Data;
using System.Text.RegularExpressions;

namespace SpiderZYM
{
    public class SpiderTaobao
    {
        const string urlBase = "http://s.taobao.com/search?q=%D7%D4%D3%C9%C3%C5&ex_q=%B5%AF%BB%C9&filterFineness=&atype=&commend=all&ssid=s5-e";
        static NodeList _linkResult;
        static CookieContainer cc = new CookieContainer();
        static NodeList _itemLink = new NodeList();
        const string pathBase = "products";

        public NodeList LinkResult
        {
            get
            {
                return _linkResult;
            }
        }

        public NodeList ItemLink
        {
            get
            {
                return _itemLink;
            }
        }

        public SpiderTaobao()
        {
        }

        public void Start()
        {
            InitPage();
            GetPageInfo();
            GetItemInfo();            
        }

        private void GetPageInfo()
        {
            NodeList result = _linkResult;

            int length = result.Count;
            Console.WriteLine("合计搜索到{0}页信息", length);

            for (int i = 0; i < length; i++)
            {
                ATag a = result[i] as ATag;
                a.Link = "http://s.taobao.com" + a.Link;
                GetLinkForPage(a.Link);
            }
        }

        public void InitPage()
        {
            Lexer lexer = new Lexer(GetHtml(urlBase));
            Parser parse = new Parser(lexer);
            parse.Encoding = "gb2312";
            NodeFilter linkFilter = new LinkRegexFilter(@"s\=\d+\#J_FilterTabBar");
            _linkResult = parse.Parse(linkFilter);
        }

        private void GetItemInfo()
        {
            Console.WriteLine("合计搜索到{0}条商品信息", _itemLink.Count);

            NodeList result = _itemLink;

            int length = result.Count;

            for (int i = 0; i < length; i++)
            {                
                ATag a = result[i] as ATag;
                Console.WriteLine("正在获取:{0}",a.LinkText);
                ParserDetailPage(a.Link);
                Console.WriteLine("成功获取:{0}", a.LinkText);
            }

            Console.WriteLine("合计获取到{0}条商品信息", _itemLink.Count);
        }

        public void GetLinkForPage(string url)
        {
            Lexer lexer = new Lexer(GetHtml(url));
            Parser parse = new Parser(lexer);
            parse.Encoding = "gb2312";           
            NodeFilter linkFilter = new LinkRegexFilter(@"^http\://item\.taobao\.com/item\.htm\?id\=\d+$");
            NodeFilter classFilter = new HasAttributeFilter("class", "EventCanSelect");
            AndFilter andFilter = new AndFilter(linkFilter, classFilter);
            NodeList result = parse.Parse(andFilter);

            int length = result.Count;
            for (int i = 0; i < length; i++)
            {                
                ItemLink.Add(result[i]);
            }            
        }

        public void ParserDetailPage(string url)
        {
            string id = Regex.Match(url,@"id\=(?<id>\d+)").Groups["id"].Value;
            string html = GetHtml(url);
            //string desc = GetDetailForDetailHtml(html);
            string prductPath = pathBase + "/" + id;

            if (!Directory.Exists(pathBase))
            {
                Directory.CreateDirectory(pathBase);
            }

            //SaveDetailText(prductPath, desc);

            NodeList result = GetDetailPageForHtml(html);
            NodeList pictures = GetPicturesForDetailHtml(result);
            DownloadPictures(pathBase, pictures);
        }
        
        public NodeList GetDetailPageForHtml(string html)
        {
            Parser parse = GetParser(html);

            NodeFilter showidFilter = new HasAttributeFilter("id", "detail");
            NodeFilter showclassFilter = new HasAttributeFilter("class", "box");
            AndFilter showFilter = new AndFilter(showidFilter, showclassFilter);

            NodeFilter contentidFilter = new HasAttributeFilter("id", "J_DivItemDesc");
            NodeFilter contentclassFilter = new HasAttributeFilter("class", "content");
            AndFilter contentFilter = new AndFilter(contentidFilter, contentclassFilter);

            OrFilter orFitler = new OrFilter(showFilter, contentFilter);

            return parse.Parse(orFitler);
        }

        private static Parser GetParser(string html)
        {
            Lexer lexer = new Lexer(html);
            Parser parse = new Parser(lexer);
            parse.Encoding = "gb2312";
            return parse;
        }
        
        public string GetDetailForDetailHtml(string html)
        {
            Match mc = Regex.Match(html, @"apiItemDesc\u0022\:\u0022(?<url>http\://dsc03\.taobao\.com/[^\u0022]+)");
            if (mc.Success && mc.Groups["url"].Success)
            {
                string url = mc.Groups["url"].Value;
                string detailHtml = GetHtml(url);
                detailHtml = Regex.Replace(detailHtml, @"(var desc\=')|(';)|(\\$)", "", RegexOptions.Multiline);
                detailHtml = Regex.Replace(detailHtml,"&nbsp", " ");
                detailHtml = Regex.Replace(detailHtml, "(; +)", ";");
                detailHtml = "<div>" + detailHtml + "</div>";
                Parser parser = GetParser(detailHtml);
                return parser.Parse(null)[0].ToPlainTextString();
            }

            return "";
        }

        public void SaveDetailText(string path, string desc)
        {
            string filename = path + "/描述.txt";
            if (File.Exists(filename))
            {
                return;
            }

            StreamWriter writer = new StreamWriter(filename);
            writer.Write(desc);
            writer.Flush();
            writer.Close();
        }

        public NodeList GetPicturesForDetailHtml(NodeList result)
        {
            NodeFilter imgFilter = new NodeClassFilter(typeof(ImageTag));
            XorFilter xorFilter = new XorFilter();
            string[] s = new string[] { "http://a.tbcdn.cn/sys/common/icon/btn/add_to_share.png", "http://img04.taobaocdn.com/tps/i4/T1qU4sXiXxXXXXXXXX-114-25.png" };
            for(int i=0;i<s.Length;i++){
                if(i==0){
                    xorFilter = new XorFilter(imgFilter, new HasAttributeFilter("src", s[i]));
                }else{
                    xorFilter = new XorFilter(xorFilter, new HasAttributeFilter("src", s[i]));
                }
            }

            NodeList imgResult = result.ExtractAllNodesThatMatch(xorFilter, true);
            return imgResult;
        }

        public void DownloadPictures(string path,NodeList result)
        {
            int length = result.Count;
            for (int i = 0; i < length; i++)
            {
                ImageTag img = result[i] as ImageTag;
                DownloadPicture(path,img.ImageURL);
            }
        }

        public string GetHtml(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);  //构造请求
            request.KeepAlive = true;
            request.CookieContainer = cc;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
            Stream newStream = request.GetRequestStream();
            newStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //构造应答
            Stream stream = response.GetResponseStream();
            string result = new StreamReader(stream, System.Text.Encoding.GetEncoding("gb2312")).ReadToEnd();
            response.Close();

            return result;
        }

        public void DownloadPicture(string path, string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);  //构造请求
            request.KeepAlive = true;
            request.CookieContainer = cc;
            request.Method = "GET";
            request.ContentType = "text/html";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
            request.Referer = url;

            HttpWebResponse response = null;

            bool isbool = false;

            while (!isbool)
            {
                try
                {
                    response = (HttpWebResponse)request.GetResponse();  //构造应答
                    isbool = true;
                }
                catch
                {

                }               
            }            

            Stream stream = response.GetResponseStream();
            int length = (int)response.ContentLength;
            BinaryReader breader = new BinaryReader(stream);

            Match mc = Regex.Match(url, "[^/]+.jpg$", RegexOptions.IgnoreCase);
            if (!mc.Success)
            {
                return;
            }

            string filename = path + @"\" + mc.Value;

            if (File.Exists(filename))
            {
                breader.Close();
                response.Close();
                return;
            }

            try
            {
                FileStream fs = new FileStream(filename, FileMode.Create);
                fs.Write(breader.ReadBytes(length), 0, length);

                fs.Flush();
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0},{1}",filename,ex.Message);
            }

            breader.Close();
            response.Close();
        }
    }
}
