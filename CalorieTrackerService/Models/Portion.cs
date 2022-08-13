using System.ComponentModel.DataAnnotations;

namespace MvcCalorie.Models
{
    // To Add the SQL server go to Tools - > Package Manager Console
    //
    // Install-Package Microsoft.EntityFrameworkCore.Design
    // Install-Package Microsoft.EntityFrameworkCore.SqlServer
    //
    // Then Scaffold the pages
    //
    // Then initial Migration
    //
    // Add-Migration InitialCreate
    // Update-Database
    //
    // For some reason needed this one time:
    // Microsoft.VisualStudio.Web.CodeGeneration.Design
    //

    // To reset ( dont know if it works )
    //
    // Update-Database -Migration 0
    // Remove-Migration
    // Add-Migration InitialCreate
    // Update-Database
    //

    public class Portion
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }
        public string? Product { get; set; }
        public int Calories { get; set; }
        public string? UserToken { get; set; }

        public string? Photo { get; set; }

        public Portion() { }
        public Portion(PortionDTOSend portionItem) =>
        (Product, Id, Date, Time, Calories,Photo,UserToken) = (portionItem.Product, portionItem.DatabaseId, DateTime.Parse(portionItem.Date), DateTime.Parse(portionItem.Time), portionItem.Calories, portionItem.Photo, portionItem.UserToken);

    }

    public class PortionDTOSend
    {
        public int RecID { get; set; }
        public string? Product { get; set; }
        public int DatabaseId { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public int Calories { get; set; }
        public string? UserToken { get; set; }

        public string? Photo { get; set; }

    }


    public class PortionDTO
    {
        public int Id { get; set; }
        public string? Product { get; set; }

        public PortionDTO() { }
        public PortionDTO(Portion portionItem) =>
        (Id) = (portionItem.Id);
    }
}
