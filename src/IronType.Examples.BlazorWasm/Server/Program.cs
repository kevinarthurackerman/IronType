var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddIronTypeCore(x => x.WithAssemblyTypeMappings(typeof(Program)));

builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseIronType();
    x.UseSqlite("FileName=app.db");
});

builder.Services.AddControllers()
    .AddNewtonsoftJson(x => x.SerializerSettings.UseIronType());

builder.Services.AddSwaggerGen(x => x.UseIronType());

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

app.UseCors(x => x.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseSwagger().UseSwaggerUI();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var jsonOpts = scope.ServiceProvider.GetRequiredService<IOptions<JsonOptions>>();

    db.Database.EnsureCreated();

    var order = new Order
    {
        Id = new OrderId(Guid.NewGuid()),
        CustomerName = "Test",
        OrderedOn = SystemClock.Instance.GetCurrentInstant().InUtc().Date,
        Location = new Location(123.456m,789.012m),
        Height = Length.FromFeet(1),
        Length = Length.FromMeters(1),
        Width = Length.FromAngstroms(1),
        Weight = Mass.FromTonnes(1)
    };

    db.Add(order);

    db.SaveChanges();
}

app.Run();