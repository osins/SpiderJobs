using System;
using System.Collections.Generic;
using System.Text;
using SpiderDomain;

namespace SpiderDataAccess
{
    public class JobMap
    {
        public static void Insert(Job param)
        {
            MapperHelper.Instance().Insert("JobMap.InsertJob", param);
        }

        public static bool IsExist(string url)
        {
            return MapperHelper.Instance().QueryForObject("JobMap.IsExist", url) != null;
        }
    }
}
