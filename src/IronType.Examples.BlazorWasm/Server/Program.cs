var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddIronType(x =>
{
    x.UseNodaTime();
    // x.UseJson();
    x.TypeData.Add(SimpleTypeDataFactory.Create<OrderId, Guid>());
});

builder.Services.UseIronTypeJson();

builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseIronType();
    x.UseSqlite("FileName=app.db");
});

builder.Services.AddControllers(); //.AddJsonOptions(x => x.JsonSerializerOptions.UseIronType());

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

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

using (var scope = app.Services.CreateScope())
using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
{
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
    dbContext.Database.Migrate();

    var order = new Order
    {
        Id = new OrderId(Guid.NewGuid()),
        OrderedOn = LocalDate.FromDateTime(DateTime.Now),
        CustomerName = "John Doe"
    };

    dbContext.Add(order);

    dbContext.SaveChanges();

    var persistedOrder = dbContext.Orders.FirstOrDefault();
}

app.Run();
