var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSharedServices();

builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseIronType();
    x.UseSqlite("FileName=app.db");
});

builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.UseIronType());

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

app.MapControllers();

app.Run();
