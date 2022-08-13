using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CalorieTracker.Models;

namespace CalorieTracker
{
    public class CalorieTrackerDatabase
    {
        readonly SQLiteConnection database;

        public PortionItem PortionRecord;        // Instance of the current loaded

        public SettingsItem SettingsRecord;


        public void LoadSettings()
        {
            if (SettingsRecord == null)
            {
                SettingsRecord = database.Table<SettingsItem>().Where(i => i.RecID == 1).FirstOrDefault();
                if (SettingsRecord == null)
                {
                    SettingsRecord = new SettingsItem
                    {
                        UserToken = App.GetApp.b_test ? "JAN" : "",
                        AdminPassword = "",
                        RecID = 1,
                        ServerURL = "https://192.168.137.15:7254"
                    };
                    database.Insert(SettingsRecord);
                }
            }
        }
        public void SaveSettings()
        {
            database.Update(SettingsRecord);
        }
        public CalorieTrackerDatabase(string dbPath)
        {
            database = new SQLiteConnection(dbPath);

            database.CreateTable<PortionItem>();
            database.CreateTable<SettingsItem>();

            LoadSettings();

            if (App.GetApp.b_test) CreateTestData();
        }
        public int SavePortionRecord()
        {
            if (PortionRecord == null)
                return 0;

            if (PortionRecord.UserToken == "") PortionRecord.UserToken = SettingsRecord.UserToken;

            PortionRecord.Day = int.Parse(PortionRecord.Date.Substring(0, 2));
            PortionRecord.Month = int.Parse(PortionRecord.Date.Substring(3, 2));
            PortionRecord.Year = int.Parse(PortionRecord.Date.Substring(6, 4));

            if (PortionRecord.RecID != 0)
                return database.Update(PortionRecord);
            else
                return database.Insert(PortionRecord);
        }
        public int SavePortionRecord(PortionItem p)
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
            database.Execute("DELETE FROM [PortionItem]");
        }
        public List<PortionItem> GetPortionsByDateRange(string date_from, string date_to)
        {
            List<PortionItem> ret = new List<PortionItem>();

            DateTime d_from = DateTime.ParseExact(date_from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime d_to = DateTime.ParseExact(date_to, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            while (d_from <= d_to)
            {
                string s_from = d_from.ToShortDateString();
                if (SettingsRecord.Admin)
                    ret.AddRange(database.Table<PortionItem>().Where(i => i.Date == s_from).ToList());
                else
                    ret.AddRange(database.Table<PortionItem>().Where(i => i.Date == s_from && i.UserToken == SettingsRecord.UserToken).ToList());
                d_from = d_from.AddDays(1);
            };
            return ret;
        }
        public List<PortionItem> GetUnsentPortions()
        {
            return database.Table<PortionItem>().Where(i => i.Sent == false && i.Completed == true).ToList();
        }
        public PortionItem GetPortionByID(int ID)
        {
            return database.Table<PortionItem>().Where(i => i.RecID == ID).FirstOrDefault();
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
            PortionRecord = new PortionItem
            {
                UserToken = SettingsRecord.UserToken,
                Product = "",
                Completed = false,
                Sent = false,
                Date = DateTime.Today.ToShortDateString(),
                Time = DateTime.Now.ToShortTimeString()
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
                    int item = rand.Next(0, food_items.Count - 1);
                    string user_token = "";
                    switch (rand.Next(0, 2))
                    {
                        case 0: user_token = "JAN"; break; // 2 test users
                        case 1: user_token = "TIM"; break;
                    }
                    PortionRecord = new PortionItem
                    {
                        Date = s_date,
                        Time = rand.Next(8, 19).ToString().PadLeft(2, '0') + ":" + rand.Next(0, 59).ToString().PadLeft(2, '0'),
                        Product = food_items[item].p,
                        Calories = food_items[item].c,
                        Completed = true,
                        Sent = true,
                        UserToken = user_token
                    };
                    SavePortionRecord();
                }
                date_ref = date_ref.AddDays(-1);
            }
        }
    }
}
