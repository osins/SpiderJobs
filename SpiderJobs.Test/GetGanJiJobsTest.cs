using SpiderJobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser;
using System;
using Winista.Text.HtmlParser.Tags;
using SpiderDomain;

namespace SpiderJobs.Test
{
    
    
    /// <summary>
    ///这是 GetGanJiJobsTest 的测试类，旨在
    ///包含所有 GetGanJiJobsTest 单元测试
    ///</summary>
    [TestClass()]
    public class GetGanJiJobsTest
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

        #region 附加测试属性
        // 
        //编写测试时，还可使用以下属性:
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
        ///GetListUrl 的测试
        ///</summary>
        [TestMethod()]
        public void GetListUrlTest()
        {
            GetGanJiJobs target = new GetGanJiJobs(); // TODO: 初始化为适当的值
            string url = "http://sh.ganji.com/jzcuxiaoyuan/"; // TODO: 初始化为适当的值

            NodeList actual;
            actual = target.GetListUrl(url);
            if (actual == null)
            {
                return;
            }
            
            for (int i=0;i<actual.Count;i++)
            {
                ATag a = actual[i] as ATag;
                if (a.Link.IndexOf("#") <= 0)
                {
                    Console.WriteLine(a.Link);
                }
            }

            Console.WriteLine("Count:{0}", actual.Count);
        }

        /// <summary>
        ///GetDetail 的测试
        ///</summary>
        [TestMethod()]
        public void GetDetailTest()
        {
            GetGanJiJobs target = new GetGanJiJobs(); // TODO: 初始化为适当的值
            string url = "http://sh.ganji.com/jzcuxiaoyuan/10070810_1244906.htm"; // TODO: 初始化为适当的值
            NodeList expected = null; // TODO: 初始化为适当的值
            Job actual;
            actual = target.GetDetail(url);

            Console.WriteLine(actual.description);
            Console.WriteLine(actual.company);
        }
    }
}
