using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CalorieTracker
{
    public class DataFunctions
    {
        readonly SQLiteConnection database;

        public PortionTable PortionRecord;        // Instance of the current loaded

        public SettingsTable SettingsRecord;

        public void LoadSettings()
        {
            if (SettingsRecord == null)
            {
                SettingsRecord = database.Table<SettingsTable>().Where(i => i.RecID == 1).FirstOrDefault();
                if (SettingsRecord == null)
                {
                    SettingsRecord = new SettingsTable();
                    SettingsRecord.user_token = App.GetApp.b_test?"JAN":"";
                    SettingsRecord.admin_password = "";
                    SettingsRecord.RecID = 1;
                    SettingsRecord.server_url = "http://192.168.137.166";
                    database.Insert(SettingsRecord);
                }
            }
        }
        public void SaveSettings()
        {
            database.Update(SettingsRecord);
        }
        public DataFunctions(string dbPath)
        {
            database = new SQLiteConnection(dbPath);

            database.CreateTable<PortionTable>();
            database.CreateTable<SettingsTable>();

            LoadSettings();

            if(App.GetApp.b_test)CreateTestData();
        }
        public int SavePortionRecord()
        {
            if (PortionRecord == null)
                return 0;

            if(PortionRecord.user_token=="") PortionRecord.user_token = SettingsRecord.user_token;

            PortionRecord.day = int.Parse(PortionRecord.date.Substring(0, 2));
            PortionRecord.month = int.Parse(PortionRecord.date.Substring(3, 2));
            PortionRecord.year = int.Parse(PortionRecord.date.Substring(6, 4));

            if (PortionRecord.RecID != 0)
                return database.Update(PortionRecord);
            else
                return database.Insert(PortionRecord);
        }
        public int SavePortionRecord(PortionTable p)
        {
            PortionRecord = p;
            return SavePortionRecord();
        }
        public void DeleteCurrentPortionRecord()
        {
            if (PortionRecord.RecID > 0)
            {
                App.Files.DeleteFile(PortionRecord.RecID.ToString() + ".jpg");
                database.Delete(PortionRecord);
            }
        }
        public void DeleteAllPortionRecords()
        {
            database.Execute("DELETE FROM [PortionTable]");
        }
        public List<PortionTable> GetPortionsByDateRange(string date_from, string date_to)
        {
            List<PortionTable> ret = new List<PortionTable>();

            DateTime d_from = DateTime.ParseExact(date_from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime d_to = DateTime.ParseExact(date_to, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            while(d_from <= d_to)
            {
                string s_from = d_from.ToShortDateString();
                if(SettingsRecord.b_admin)
                    ret.AddRange(database.Table<PortionTable>().Where(i => i.date == s_from).ToList());
                else
                    ret.AddRange(database.Table<PortionTable>().Where(i => i.date == s_from && i.user_token == SettingsRecord.user_token).ToList());
                d_from = d_from.AddDays(1);
            };
            return ret;
        }
        public List<PortionTable> GetUnsentPortions()
        {
            return database.Table<PortionTable>().Where(i => i.b_sent == false && i.b_completed == true).ToList();
        }
        public PortionTable GetPortionByID(int ID)
        {
            return database.Table<PortionTable>().Where(i => i.RecID == ID).FirstOrDefault();
        }
        private class food_item
        {
            public string p;    // product
            public int c;    // calories
            public food_item(string _p, int _c)
            {
                p = _p;
                c = _c;
            }
        }
        public void CreatePortionRecord()
        {
            PortionRecord = new PortionTable
            {
                user_token = SettingsRecord.user_token,
                product = "",
                b_completed = false,
                b_sent = false,
                date = DateTime.Today.ToShortDateString(),
                time = DateTime.Now.ToShortTimeString()
            };
            SavePortionRecord();
        }
        public void CreateTestData()
        {
            DateTime date_ref = DateTime.Today;

            var rand = new Random();

            List<food_item> food_items = new List<food_item>
            {
                new food_item("Bannana", 89),
                new food_item("Cheese Sandwitch", 467),
                new food_item("Chocolate Bar", 595),
                new food_item("Vegetable Soup", 186),
                new food_item("Fish and Chips", 988),
                new food_item("Can of Coke", 120)
            };

            // Create test data for the past 15 days
            for (int j = 0; j < 15; j++)
            {
                string s_date = date_ref.ToShortDateString();
                for (int i = 0; i < rand.Next(6, 10); i++)
                {
                    PortionRecord = new PortionTable();

                    PortionRecord.date = s_date;
                    PortionRecord.time = rand.Next(8,19).ToString().PadLeft(2, '0')+":"+ rand.Next(0, 59).ToString().PadLeft(2, '0'); 

                    int item = rand.Next(0, food_items.Count - 1);
                    PortionRecord.product = food_items[item].p;
                    PortionRecord.calories = food_items[item].c;
                    switch (rand.Next(0,2))
                    {
                        case 0: PortionRecord.user_token = "JAN"; break; // 2 test users
                        case 1: PortionRecord.user_token = "TIM"; break;
                    }
                    PortionRecord.b_completed = true;
                    PortionRecord.b_sent = true;
                    SavePortionRecord();
                }
                date_ref = date_ref.AddDays(-1);
            }
        }
    }
}
