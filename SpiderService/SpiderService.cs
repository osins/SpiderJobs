using System;
using System.Timers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using SpiderJobs;

namespace SpiderService
{
    public partial class SpiderService : ServiceBase
    {
        public Timer timer = new Timer();
        public bool isFirst = true;
        public static int count = 0;

        public SpiderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(FirstTimerElapsedEvent);
            timer.Interval = 60000;
            timer.AutoReset = false;
            timer.Start();

            GC.KeepAlive(timer);
        }

        protected override void OnStop()
        {

        }

        protected void FirstTimerElapsedEvent(object source, ElapsedEventArgs e)
        {
            SpiderEventLog.WriteSourceLog("Spider 1010兼职网站爬取程序第1次爬取开始", "Spider 1010兼职网站爬取程序第1次爬取开始", EventLogEntryType.Warning);

            SpiderRun();

            if (isFirst)
            {
                timer.Elapsed += new ElapsedEventHandler(TimerElapsedEvent);
                timer.Interval = 1800000;
                timer.AutoReset = true;
                timer.Start();

                GC.KeepAlive(timer);
                isFirst = false;
            }
        }

        protected void TimerElapsedEvent(object source, ElapsedEventArgs e)
        {
            count = count + 1;

            string eventString = "Spider 1010兼职网站爬取程序第" + count.ToString() + "次爬取开始";

            SpiderEventLog.WriteSourceLog(eventString, eventString, EventLogEntryType.Warning);
            SpiderRun();
        }

        private static void SpiderRun()
        {
            SpiderEventLog.WriteSourceLog("Spider 1010兼职网站爬取程序已启动", "Spider 1010兼职网站爬取程序已启动", EventLogEntryType.Warning);

            Start1010JobsSpider spider1010 = new Start1010JobsSpider();
            spider1010.Run();

            SpiderEventLog.WriteSourceLog("Spider 1010兼职网站爬取程序已结束", "Spider 1010兼职网站爬取程序已结束", EventLogEntryType.Warning);
        }
    }
}
