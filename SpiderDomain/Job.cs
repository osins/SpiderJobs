using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderDomain
{
    public class Job
    {
        public int id
        {
            get;
            set;
        }

        public int type_id
        {
            get;
            set;
        }

        public int category_id
        {
            get;
            set;
        }

        public string title
        {
            get;
            set;
        }

        public string description
        {
            get;
            set;
        }

        public string company
        {
            get;
            set;
        }

        public int city_id
        {
            get;
            set;
        }

        //public string url
        //{
        //    get;
        //    set;
        //}

        public string sp1010url
        {
            get;
            set;
        }

        public DateTime created_on
        {
            get;
            set;
        }

        public bool is_active
        {
            get;
            set;
        }

        public string poster_email
        {
            get;
            set;
        }

        public string tel
        {
            get;
            set;
        }
    }
}
