using SpiderJobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Winista.Text.HtmlParser;
using System;
using SpiderDomain;
using Winista.Text.HtmlParser.Tags;

namespace SpiderJobs.Test
{
    
    
    /// <summary>
    ///这是 Get1010JobsTest 的测试类，旨在
    ///包含所有 Get1010JobsTest 单元测试
    ///</summary>
    [TestClass()]
    public class Get1010JobsTest
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
        ///GetJobInfoParser 的测试
        ///</summary>
        [TestMethod()]
        public void GetJobInfoParserTest()
        {
            Category catalog = new Category(); // TODO: 初始化为适当的值
            Get1010Jobs target = new Get1010Jobs(catalog); // TODO: 初始化为适当的值
            string url = "http://sh.1010jz.com/html/shanghai/011_1522688.html"; // TODO: 初始化为适当的值
            Job expected = null; // TODO: 初始化为适当的值
            Job actual;
            ATag node = new ATag();
            node.Link = url;
            actual = target.GetJobInfoParser(url);
            Console.WriteLine("title:{0}", actual.title);
            Console.WriteLine("email:{0}", actual.poster_email);
        }

        /// <summary>
        ///IsExistJob 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SpiderJobs.dll")]
        public void IsExistJobTest()
        {
            string url = "fdsfsd"; // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = Get1010Jobs_Accessor.IsExistJob(url);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }
    }
}
