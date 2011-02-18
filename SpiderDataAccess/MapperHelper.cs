using System;
using System.Collections.Generic;
using System.Text;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System.Configuration;

namespace SpiderDataAccess
{
    public class MapperHelper
    {
        #region Fields
        private static ISqlMapper _Mapper = null;
        #endregion

        /// <summary>
        /// static Configure constructor that can be
        /// used for callback
        /// </summary>
        /// <param name="obj"></param>
        protected static void Configure(object obj)
        {
            _Mapper = null;
        }

        /// <summary>
        /// Init the 'default' SqlMapper defined by the SqlMap.Config file.
        /// </summary>
        protected static void InitMapper()
        {
            DomSqlMapBuilder builder = new DomSqlMapBuilder();
            string config = ConfigurationManager.AppSettings.Get("sqlmapconfig");
            _Mapper = builder.Configure(config);
        }

        /// <summary>
        /// Get the instance of the SqlMapper defined by the SqlMap.Config file.
        /// </summary>
        /// <returns>A SqlMapper initalized via the SqlMap.Config file.</returns>
        public static ISqlMapper Instance()
        {
            if (_Mapper == null)
            {
                lock (typeof(SqlMapper))
                {
                    if (_Mapper == null) // double-check
                    {
                        InitMapper();
                    }
                }
            }
            return _Mapper;
        }
    }
}
