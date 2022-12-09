var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddIronType(x =>
{
    x.UseNodaTime();
    x.TypeData.Add(SimpleTypeDataFactory.Create<OrderId, Guid>());
});

builder.Services.AddDbContext<AppDbContext>((sp, x) =>
{
    x.UseIronType();
    x.UseSqlite("FileName=app.db");
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
{
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
    dbContext.Database.Migrate();

    var order = new Order
    {
        Id = new OrderId(Guid.NewGuid()),
        OrderedOn = LocalDate.FromDateTime(DateTime.Now)
    };

    dbContext.Add(order);

    dbContext.SaveChanges();

    var persistedOrder = dbContext.Orders.FirstOrDefault();
}

app.Run();
