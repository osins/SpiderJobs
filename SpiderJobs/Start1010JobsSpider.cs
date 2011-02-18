using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using SpiderDomain;
using SpiderDataAccess;
using IBatisNet.DataMapper;

namespace SpiderJobs
{
    public class Start1010JobsSpider
    {
        public void Run()
        {
            bool sign = true;

            IList<Category> catalogs = null;
            IList<City> citys = null;

            while (sign)
            {
                try
                {
                    citys = CategoryMap.GetCitys();
                    catalogs = CategoryMap.GetCategorys();
                    sign = false;
                }
                catch (Exception ex)
                {
                    SpiderEventLog.WriteWarningLog("开始加载目录地址出现数据库错误："+ex.ToString());
                    sign = true;
                }
            }

            if (catalogs == null)
            {
                return;
            }
                        
            foreach (City c in citys)
            {
                foreach (Category catalog in catalogs)
                {
                    Category newCatalog = new Category();

                    newCatalog.id = catalog.id;
                    newCatalog.name = catalog.name;
                    newCatalog.sp1010 = catalog.sp1010.Replace("sh.", c.sub_domain + ".");
                    newCatalog.city = c;

                    new Get1010Jobs(newCatalog).StartSpider();
                }
            }
        }
    }
}
