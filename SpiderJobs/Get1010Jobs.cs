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
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using SpiderDomain;
using SpiderDataAccess;

namespace SpiderJobs
{
    public class Get1010Jobs
    {
        public Category Catalog = null;

        public Get1010Jobs(Category catalog)
        {
            Catalog = catalog;
        }

        public void StartSpider()
        {
            try
            {
                for (int i = 1; i <= 5; i++)
                {

                    SpiderCurrentPage(i);

                }
            }
            catch (Exception ex)
            {
                SpiderEventLog.WriteWarningLog("HtmlParse StartSpider error:" + ex.ToString());
            }
        }

        public void SpiderCurrentPage(int idx)
        {
            ParserConf.GetConfiguration().RootPath = AppDomain.CurrentDomain.BaseDirectory;
            string url = Catalog.sp1010 + string.Format("index{0}.html", idx);
            Parser parser;
            NodeList nodeList=null;
            int count = 0;
            bool sign=true;

            while (sign && count<5)
            {
                SpiderEventLog.WriteSourceLog("Spider " + url, url, EventLogEntryType.Information);

                try
                {
                    parser = new Parser(new HttpProtocol(new Uri(url)));
                }
                catch (Exception ex)
                {
                    SpiderEventLog.WriteWarningLog("获取列表页面数据错误:" + url + "\r\n" + ex.ToString());
                    return;
                }

                if (parser == null)
                {
                    return;
                }

                sign = false;

                try
                {
                    NodeFilter filter = new HasAttributeFilter("class", "Linklist");
                    nodeList = parser.ExtractAllNodesThatMatch(filter);
                }
                catch (Exception ex)
                {
                    SpiderEventLog.WriteWarningLog("获取列表页面数据错误:" + url + "\r\n" + ex.ToString());
                    sign = true;
                }

                count++;
            }

            if (nodeList == null)
            {
                return;
            }

            int length = nodeList.Count;
            for (int i = 0; i < length; i++)
            {
                ATag node = nodeList[i] as ATag;
                if (IsExistJob(node.Link))
                {
                    SpiderEventLog.WriteLog(string.Format("职务 [{0}] 已存在",node.LinkText));
                    continue;
                }

                Job jobinfo = GetJobInfoParser(node.Link);
                jobinfo.title = Regex.Replace(node.LinkText,"&[^&;]{0,};", "",RegexOptions.IgnoreCase);

                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("=".PadLeft(120,'='));
                Console.WriteLine("title:{0}", jobinfo.title);
                Console.WriteLine("url:{0}", jobinfo.sp1010url);
                Console.WriteLine("tel:{0}", jobinfo.tel);
                Console.WriteLine("email:{0}", jobinfo.poster_email);
                Console.WriteLine("desc:{0}", jobinfo.description);
                Console.WriteLine("=".PadLeft(120,'='));

                Console.ForegroundColor = color;

                InsertJobInfo(jobinfo);
            }
        }

        private static bool IsExistJob(string url)
        {
            if (string.IsNullOrEmpty(url) || !url.EndsWith("html"))
            {
                return true;
            }

            bool sign = true;
            while (sign)
            {
                try
                {
                    return JobMap.IsExist(url);
                }
                catch (Exception ex)
                {
                    SpiderEventLog.WriteWarningLog("HtmlParse GetJobDetail error:" + ex.ToString());
                    Thread.Sleep(10000);
                    sign = true;
                }
            }

            return false;
        }

        public Job GetJobInfoParser(string url)
        {
            Job jobinfo = new Job();

            string title = string.Empty;
            string description = string.Empty;
            DateTime dt = DateTime.Now;
            string email = string.Empty;

            Parser parser = new Parser(new HttpProtocol(new Uri(url)));

            NodeFilter detail = new HasAttributeFilter("class", "d_left");

            NodeList nodeDetail = parser.ExtractAllNodesThatMatch(detail);
            if (nodeDetail == null || nodeDetail.Count == 0)
            {
                return jobinfo;
            }

            description = GetDetailString(nodeDetail);
            Match m = Regex.Match(description, @"发布时间：(?<date>\d\d\d\d-\d{1,2}\-\d{1,2} \d{1,2}\:\d{1,2})");

            dt = DateTime.Now;

            if (m.Success && m.Groups["date"].Success && DateTime.TryParse(m.Groups["date"].Value, out dt)) { }            

            Match emailMatch = Regex.Match(description, @"([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)");
            if (emailMatch.Success)
            {
                email = emailMatch.Value;
            }

            Match telMatch = Regex.Match(description, @"(1[3|5|8][0-9]|15[0|3|6|7|8|9]|18[8|9])\d{8}");
            if (telMatch.Success)
            {
                jobinfo.tel = telMatch.Value;
            }
            
            jobinfo.category_id = Catalog.id;
            jobinfo.title = title;
            jobinfo.description = description;
            jobinfo.created_on = dt;
            jobinfo.is_active = true;
            jobinfo.city_id = Catalog.city.id;
            jobinfo.sp1010url = url;
            jobinfo.poster_email = email;

            return jobinfo;
        }

        private string GetDetailString(NodeList nodeDetail)
        {
            string detailHtml = nodeDetail.ToHtml();
            ReplaceNode(ref detailHtml, nodeDetail, typeof(ScriptTag));
            ReplaceNode(ref detailHtml, nodeDetail, typeof(ATag));

            detailHtml = detailHtml.Replace("<br />", "\r\n").Replace("&nbsp;", "").Replace("&raquo;", "").Replace("1010兼职网", "135free.com").Replace("所在地：", "");
            detailHtml = Regex.Replace(detailHtml, @"信息编号：\d{0,}", "");  

            Lexer lexer = new Lexer(detailHtml);
            Parser parser = new Parser(lexer);
            NodeList list = parser.ExtractAllNodesThatMatch(new NodeClassFilter(typeof(Div)));
            if (list.Count > 0)
            {
                return list[0].ToPlainTextString();
            }
            else
            {
                return "";
            }
        }

        private void ReplaceNode(ref string html, NodeList nodeList, Type tag)
        {
            NodeList nodeA = nodeList.SearchFor(tag, true);

            int length = nodeA.Count;
            for (int i = 0; i < length; i++)
            {
                html = html.Replace(nodeA[i].ToHtml(), "");
            }
        }

        private void InsertJobInfo(Job jobinfo)
        {
            if (string.IsNullOrEmpty(jobinfo.title) || string.IsNullOrEmpty(jobinfo.description))
            {
                return;
            }

            bool sign = true;
            while (sign)
            {
                try
                {
                    JobMap.Insert(jobinfo);

                    sign = false;
                }
                catch (Exception ex)
                {
                    SpiderEventLog.WriteWarningLog("HtmlParse InsertJobInfo error:" + ex.ToString());

                    Thread.Sleep(3000);
                    sign=true;
                }
            }
        }
    }
}