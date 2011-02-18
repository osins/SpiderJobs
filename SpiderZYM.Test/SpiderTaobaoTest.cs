using SpiderZYM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Tags;
using Winista.Text.HtmlParser.Data;

namespace SpiderZYM.Test
{
    
    
    /// <summary>
    ///这是 SpiderTaobaoTest 的测试类，旨在
    ///包含所有 SpiderTaobaoTest 单元测试
    ///</summary>
    [TestClass()]
    public class SpiderTaobaoTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///GetHtml 的测试
        ///</summary>
        [TestMethod()]
        public void GetHtmlTest()
        {
            SpiderTaobao target = new SpiderTaobao(); // TODO: 初始化为适当的值
            string url = "http://s.taobao.com/search?q=%CA%B5%C4%BE+%D7%D4%D3%C9%C3%C5&ex_q=&filterFineness=&atype=&fs=1&commend=all&ssid=s5-e"; // TODO: 初始化为适当的值
            string expected = string.Empty; // TODO: 初始化为适当的值
            string actual;
            actual = target.GetHtml(url);
            Console.WriteLine(actual);
        }

        /// <summary>
        ///Init 的测试
        ///</summary>
        [TestMethod()]
        public void InitPage()
        {
            SpiderTaobao target = new SpiderTaobao(); // TODO: 初始化为适当的值
            target.InitPage();
            Console.WriteLine(target.LinkResult.Count);
            NodeList result = target.LinkResult;

            int length = result.Count;

            for (int i = 0; i < length; i++)
            {
                ATag a = result[i] as ATag;
                Console.WriteLine("{0} , {1}",a.Link,a.ToPlainTextString());
            }
        }

        /// <summary>
        ///Init 的测试
        ///</summary>
        [TestMethod()]
        public void Init()
        {
            SpiderTaobao target = new SpiderTaobao(); // TODO: 初始化为适当的值
            target.Start();
            NodeList result = target.ItemLink;

            int length = result.Count;
            Console.WriteLine(length);
            for (int i = 0; i < length; i++)
            {
                ATag a = result[i] as ATag;
                Console.WriteLine("{0} , {1}", a.Link, a.ToPlainTextString());
            }
        }

        /// <summary>
        ///GetPageForLink 的测试
        ///</summary>
        [TestMethod()]
        public void GetDetailForLinkTest()
        {
            SpiderTaobao target = new SpiderTaobao(); // TODO: 初始化为适当的值
            string url = "http://item.taobao.com/item.htm?id=6079437206"; // TODO: 初始化为适当的值
            string html = target.GetHtml(url);
            NodeList result = target.GetDetailPageForHtml(html);

            Console.WriteLine(result.ToHtml());

            NodeList pictures = target.GetPicturesForDetailHtml(result);

            int length = pictures.Count;
            for (int i = 0; i < length; i++)
            {
                Console.WriteLine("{0} , {1}",pictures[0].ToPlainTextString(), pictures[i].ToHtml());
            }

            target.DownloadPictures("products/6079437206/",pictures);

            Console.WriteLine(target.GetDetailForDetailHtml(html));
        }

        /// <summary>
        ///ParserDetailPage 的测试
        ///</summary>
        [TestMethod()]
        public void ParserDetailPageTest()
        {
            SpiderTaobao target = new SpiderTaobao(); // TODO: 初始化为适当的值
            string url = "http://item.taobao.com/item.htm?id=6079437206"; // TODO: 初始化为适当的值
            target.ParserDetailPage(url);
        }

        /// <summary>
        ///Init 的测试
        ///</summary>
        [TestMethod()]
        public void InitTest()
        {
            SpiderTaobao target = new SpiderTaobao(); // TODO: 初始化为适当的值
            target.Start();
            Assert.Inconclusive("无法验证不返回值的方法。");
        }
    }
}

