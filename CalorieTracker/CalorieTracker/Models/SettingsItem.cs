using SQLite;

namespace CalorieTracker.Models
{
    public class SettingsItem
    {
        [PrimaryKey, AutoIncrement]
        public int RecID { get; set; }
        public string UserToken { get; set; }
        public string AdminPassword { get; set; }
        public bool Admin { get; set; }
        public string ServerURL { get; set; }
    }
}
