using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeM8
{
    public class AppConfig
    {
        [PrimaryKey]
        public int ConfigID { get; set; }
        public string ApplicationConfiguration { get; set; }
        public int ConfigInt { get; set; }
    }
}
