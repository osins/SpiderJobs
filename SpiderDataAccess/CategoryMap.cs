using System;
using System.Collections.Generic;
using System.Text;
using SpiderDomain;

namespace SpiderDataAccess
{
    public class CategoryMap
    {
        public static IList<Category> GetCategorys(){
            return MapperHelper.Instance().QueryForList<Category>("CategoryMap.GetCategorys", null);
        }

        public static IList<City> GetCitys()
        {
            return MapperHelper.Instance().QueryForList<City>("CategoryMap.GetCitys", null);
        }
    }
}
