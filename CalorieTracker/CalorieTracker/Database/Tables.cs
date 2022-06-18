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
        public bool Deleted { get; set; }
        public bool Completed { get; set; }
        public bool Sent { get; set; }
        public string Date { get; set; }
        public int Day { get; set; } // Having to store date like this because SQL Lite does not support DateTime
        public int Month { get; set; }
        public int Year { get; set; }
        public string Time { get; set; }
        public string Product { get; set; }
        public int Calories { get; set; }
        public string UserToken { get; set; }
        public string DatabaseID { get; set; } // Id at the server side, set by result of sending
    }

    public class SettingsTable
    {
        [PrimaryKey, AutoIncrement]
        public int RecID { get; set; }
        public string UserToken { get; set; }
        public string AdminPassword { get; set; }
        public bool Admin { get; set;  }
        public string ServerURL { get; set; }

    }
}
