using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalorieTracker
{
    public class PortionTable 
    {
        [PrimaryKey, AutoIncrement]
        public int RecID { get; set; }
        public bool b_deleted { get; set; }
        public bool b_completed { get; set; }
        public bool b_sent { get; set; }
        public string date { get; set; }
        public int day { get; set; } // Having to store date like this because SQL Lite does not support DateTime
        public int month { get; set; }
        public int year { get; set; }
        public string time { get; set; }
        public string product { get; set; }
        public int calories { get; set; }
        public string user_token { get; set; }
        public string database_ID { get; set; } // Id at the server side, set by result of sending
    }

    public class SettingsTable
    {
        [PrimaryKey, AutoIncrement]
        public int RecID { get; set; }
        public string user_token { get; set; }
        public string admin_password { get; set; }
        public bool b_admin { get; set;  }
        public string server_url { get; set; }

    }
}
