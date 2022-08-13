using Microsoft.EntityFrameworkCore;
using MvcCalorie.Data;
using MvcCalorie.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MvcCalorieContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MvcCalorieContext") ?? throw new InvalidOperationException("Connection string 'MvcCalorieContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapPost("/ReceivePortionJson", async (PortionDTOSend todoItemDTO, MvcCalorieContext db) =>
{
    Portion portion = await db.Portion.FindAsync(todoItemDTO.DatabaseId);

    if (portion == null)
    {
        portion = new Portion(todoItemDTO);
        db.Add(portion);
    }
    else
    {
        portion.Product = todoItemDTO.Product;
        portion.Date = DateTime.Parse(todoItemDTO.Date);
        portion.Time = DateTime.Parse(todoItemDTO.Time);
        portion.Calories = todoItemDTO.Calories;
        portion.UserToken = todoItemDTO.UserToken;
        portion.Photo = todoItemDTO.Photo;
        db.Update(portion);
    }

    await db.SaveChangesAsync();

    return Results.Created($"/ReceivePortion/{portion.Id}", new PortionDTO(portion));
});



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Portions}/{action=Index}/{id?}");

app.Run();


