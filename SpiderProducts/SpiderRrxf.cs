using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Filters;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Tags;
using System.Text.RegularExpressions;

namespace SpiderProducts
{
    public class SpiderRrxf
    {
        public string LoginURL = "http://rrxf.cn/soft/reg.php";
        public static CookieContainer cc = new CookieContainer();
        public string picturePATH = "images";
        public string pictureURL = "http://tbimg.135free.com";

        public SpiderRrxf()
        {
            
        }

        public void Parse()
        {
            string username = "gaohong"; // TODO: 初始化为适当的值
            string password = "w123456"; // TODO: 初始化为适当的值

            LoginSite(username, password);

            string html = GetHtml("http://rrxf.cn/index.php");
            List<ATag> catelogs = ParseCatelog(html);

            foreach (ATag a in catelogs)
            {                
                ParseProducts(a);
            }
        }

        public void ParseProducts(ATag a)
        {
            string html = GetHtml(a.Link.Replace("../", "http://rrxf.cn/"));

            Lexer lexer = new Lexer(html);
            Parser parser = new Parser(lexer);

            NodeFilter nav = new HasAttributeFilter("class", "photoyi");
            NodeList navNodes = parser.Parse(nav);

            if (navNodes == null)
                return;

            int length = navNodes.Count;
            for (int i = 0; i < length; i++)
            {
                ATag link = ParseProductUrl(navNodes[i].ToHtml());
                Console.WriteLine(link.Link);
                ParseProduct(link);
            }
        }

        public void ParseProduct(ATag a)
        {
            string html = GetHtml(a.Link);

            Lexer lexer = new Lexer(html);
            Parser parser = new Parser(lexer);

            NodeFilter productArea = new HasAttributeFilter("id", "productyou");
            NodeList nodes = parser.ExtractAllNodesThatMatch(productArea);

            ParseProductTitle(nodes);
            ParseProductShowPhoto(nodes);
            ParseProductDemoPhoto(nodes);
            ParsePorductDescribe(nodes);

            NodeFilter productAttributeArea = new HasAttributeFilter("class", "chans");
            NodeList productAttributeAreaNodes = nodes.ExtractAllNodesThatMatch(productAttributeArea,true);

            NodeFilter productAttributes = new HasAttributeFilter("class", "cph");
            NodeList productAttributeNodes = nodes.ExtractAllNodesThatMatch(productAttributes, true);

            int length = productAttributeNodes.Count;
            for (int i = 0; i < length; i++)
            {
                INode n = productAttributeNodes[i].Children[0];
                string t =n.ToPlainTextString();
                if (Regex.Match(t, @"^\s{0,}颜色", RegexOptions.IgnoreCase).Success)
                {
                    ParseProductColors(n);
                }
                Console.WriteLine();
            }
        }

        private static void ParseProductColors(INode n)
        {
            NodeList colors = n.Parent.Children;
            if (colors == null || colors.Count == 0)
            {
                return;
            }

            int length = colors.Count;
            for (int i = 0; i < length; i++)
            {
                Console.WriteLine(colors[i].ToPlainTextString());
            }
        }

        private static void ParseProductTitle(NodeList nodes)
        {
            NodeFilter title = new HasAttributeFilter("class", "prouductx");
            NodeList titleNodes = nodes.ExtractAllNodesThatMatch(title, true);

            Console.WriteLine(titleNodes[0].ToPlainTextString());
        }

        private void ParseProductShowPhoto(NodeList nodes)
        {
            NodeFilter show = new HasAttributeFilter("class", "Picture220");
            NodeList showNodes = nodes.ExtractAllNodesThatMatch(show, true);
            ImageTag showTag = showNodes[0] as ImageTag;
            showTag.ImageURL = showTag.ImageURL.Replace("../../", "http://rrxf.cn/");

            Console.WriteLine(showTag.ImageURL);
            DownloadPicture(showTag.ImageURL);
        }

        private void ParseProductDemoPhoto(NodeList nodes)
        {
            NodeFilter photo = new HasAttributeFilter("class", "Picture40");
            NodeList photoNodes = nodes.ExtractAllNodesThatMatch(photo, true);
            DownloadPictures(photoNodes);
        }

        private void ParsePorductDescribe(NodeList nodes)
        {
            NodeFilter miao = new HasAttributeFilter("class", "miao");
            NodeList miaoArea = nodes.ExtractAllNodesThatMatch(miao, true);

            NodeFilter pictures = new NodeClassFilter(typeof(ImageTag));
            NodeList pictureNodes = miaoArea.ExtractAllNodesThatMatch(pictures, true);

            DownloadPictures(pictureNodes);

            string miaoshu = miaoArea.AsHtml();
            miaoshu = Regex.Replace(miaoshu, @"http\://(www\.|)rrxf\.cn/", pictureURL + "/", RegexOptions.IgnoreCase);
            miaoshu = Regex.Replace(miaoshu, @"(pic|bigpic)/", "$1_", RegexOptions.IgnoreCase);
            miaoshu = miaoshu.Replace("-", "_");

            Console.WriteLine(miaoshu);
        }

        private void DownloadPictures(NodeList photoNodes)
        {
            List<ImageTag> photos = new List<ImageTag>();

            int length = photoNodes.Count;
            for (int i = 0; i < length; i++)
            {
                ImageTag imgTag = photoNodes[i] as ImageTag;
                imgTag.ImageURL = imgTag.ImageURL.Replace("../../", "http://rrxf.cn/");
                Console.WriteLine(imgTag.ImageURL);

                photos.Add(imgTag);

                DownloadPicture(imgTag.ImageURL);
            }
        }

        public void DownloadPicture(string url)
        {
            Console.WriteLine("download picture:{0}", url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);  //构造请求
            request.KeepAlive = true;
            request.CookieContainer = cc;
            request.Method = "GET";
            request.ContentType = "text/html";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
            request.Referer = url;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //构造应答
            Stream stream = response.GetResponseStream();
            int length = (int)response.ContentLength;
            BinaryReader breader = new BinaryReader(stream);
            string imagepath = picturePATH;

            if (!Directory.Exists(imagepath))
            {
                Directory.CreateDirectory(imagepath);
            }

            string filename = Regex.Replace(url,@"http\://(www\.|)rrxf\.cn/", "",RegexOptions.IgnoreCase);
            filename = filename.Replace("/", "_");
            filename = filename.Replace("-", "_");
            filename = imagepath + "/" + filename;

            FileStream fs = new FileStream(filename, FileMode.Create);
            fs.Write(breader.ReadBytes(length), 0, length);

            fs.Flush();
            fs.Close();
            breader.Close();
            response.Close();
        }

        public ATag ParseProductUrl(string html)
        {
            Lexer lexer = new Lexer(html);
            Parser parser = new Parser(lexer);

            NodeFilter filter = new LinkRegexFilter(@"lookcp\.php\?cpid\=\d{0,}");
            NodeList alist = parser.Parse(filter);
            ATag a = alist[0] as ATag;
            a.Link = "http://rrxf.cn/product/" + a.Link;
            return a;
        }

        public void LoginSite(string username, string password)
        {
            string PostData = string.Format("imageField.x=2&imageField.y=4&username=" + username + "&password=" + password);
            byte[] FinalPostData = Encoding.UTF8.GetBytes(PostData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(LoginURL);  //构造请求
            request.KeepAlive = true;
            request.CookieContainer = cc;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = PostData.Length;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
            Stream newStream = request.GetRequestStream();
            newStream.Write(FinalPostData, 0, FinalPostData.Length);   //发送请求
            newStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //构造应答
            Stream stream = response.GetResponseStream();
            string result = new StreamReader(stream, System.Text.Encoding.GetEncoding("gb2312")).ReadToEnd();
            response.Close();            
        }

        public List<ATag> ParseCatelog(string html)
        {
            List<ATag> atags = new List<ATag>();

            Lexer lexer = new Lexer(html);
            Parser parser = new Parser(lexer);

            NodeFilter nav = new HasAttributeFilter("class", "fenlei_list");
            NodeList navNodes = parser.Parse(nav);

            NodeFilter catelog = new LinkRegexFilter(@"^\.\./product/index\.php\?cplm\=\-\d\d\d\-$");
            catelog = new HasChildFilter(catelog);
            NodeList catelogNodes = navNodes[0].Children.ExtractAllNodesThatMatch(catelog);
            
            if(catelogNodes==null){
                return atags;
            }

            int length = catelogNodes.Count;
            for (int i=0;i<length;i++)
            {
                INode node = catelogNodes[i];
                ATag a = node.Children[0] as ATag;    
                atags.Add(a);
            }

            return atags;
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
    }
}
