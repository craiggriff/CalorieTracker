using SQLite;
using System;
using System.Collections.Generic;
using System.Text;


namespace CalorieTracker.Models
{
    public class PortionItem
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
        public int DatabaseId { get; set; } // Id at the server side, set by result of sending
    }

    public class PortionDTOSend
    {
        public int RecID { get; set; } 
        public string Product { get; set; }
        public int DatabaseId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public int Calories { get; set; }
        public string UserToken { get; set; }

        public string Photo { get; set; }
        public PortionDTOSend() { }
        public PortionDTOSend(PortionItem portionItem) =>
        (RecID,Product,DatabaseId,Date,Time,Calories,UserToken) = (portionItem.RecID, portionItem.Product, portionItem.DatabaseId, portionItem.Date, portionItem.Time, portionItem.Calories, portionItem.UserToken);
        
    }

    public class PortionDTO
    {
        public int Id { get; set; }
        public string Product { get; set; }

        /*
        public PortionDTO() { }
        public PortionDTO(Portion portionItem) =>
        (Id) = (portionItem.Id);
        */
    }
}
