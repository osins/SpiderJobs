using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Filters;
using System.Text.RegularExpressions;
using SpiderDomain;

namespace SpiderJobs
{
    public class GetGanJiJobs
    {
        public NodeList GetListUrl(string url)
        {
            Parser parser = ParserHelp.GetParser(url);
            NodeFilter filter = new HasAttributeFilter("class", "list_title");
            NodeList list = new NodeList();            

            list = parser.ExtractAllNodesThatMatch(filter);

            return list;
        }

        public Job GetDetail(string url)
        {
            Job info = new Job();

            Parser parser = ParserHelp.GetParser(url);

            NodeFilter miaoShu = new HasAttributeFilter("id", "miaoshu");
            NodeFilter mainBox = new HasAttributeFilter("class", "mainBox");
            NodeFilter orfilter = new OrFilter(miaoShu, mainBox);

            NodeList list = new NodeList();
            list = parser.Parse(orfilter);
            if (list == null || list.Count < 2)
            {
                return info;
            }
           
            GetMiaoShu(list, ref info);
            GetContartInfo(list, ref info);

            return info;
        }

        public void GetMiaoShu(NodeList list, ref Job info)
        {
            string miaoshu = list[1].ToPlainTextString();
            miaoshu = Regex.Replace(miaoshu, @"^[\s|\t]{1,}", "", RegexOptions.Multiline);
            info.description = miaoshu;
        }

        public void GetContartInfo(NodeList list, ref Job info)
        {
            string miaoshu = list[0].ToString();
            if (string.IsNullOrEmpty(miaoshu))
            {
                return;
            }

            miaoshu = Regex.Replace(miaoshu,@"(\\t|\s)","");

            Match company = Regex.Match(miaoshu, @"Txt\(4903\[108\,12\]\,4935\[110\,16\]\)\:\\n(?<company>\w*)\\n...End", RegexOptions.Multiline);
            if (company.Success)
            {
                info.company = company.Value;
            }
        }


    }
}
