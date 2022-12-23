namespace IronType.Examples.BlazorWasm.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TestController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public string Test()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
            _dbContext.Database.Migrate();

            var order = new Order
            {
                Id = new OrderId(Guid.NewGuid()),
                OrderedOn = LocalDate.FromDateTime(DateTime.Now),
                CustomerName = "John Doe",
                Length = Length.FromMeters(1),
                Width = Length.FromFeet(1),
                Height = Length.FromHands(20),
                Weight = Mass.FromTonnes(0.5)
            };

            _dbContext.Add(order);
            
            _dbContext.SaveChanges();

            _dbContext.ChangeTracker.Clear();
            
            var today = LocalDate.FromDateTime(DateTime.Now);
            var gt = _dbContext.Orders.Where(x => x.OrderedOn > today).FirstOrDefault();
            var gte = _dbContext.Orders.Where(x => x.OrderedOn >= today).FirstOrDefault();
            var eq = _dbContext.Orders.Where(x => x.OrderedOn == today).FirstOrDefault();
            var lte = _dbContext.Orders.Where(x => x.OrderedOn <= today).FirstOrDefault();
            var lt = _dbContext.Orders.Where(x => x.OrderedOn < today).FirstOrDefault();

            var date = _dbContext.Orders
                .Select(x => x.OrderedOn)
                .FirstOrDefault();

            var persistedOrder = _dbContext.Orders.FirstOrDefault();

            var json = JsonSerializer.Serialize(persistedOrder, new JsonSerializerOptions().UseIronType());

            var fromJson = JsonSerializer.Deserialize<Order>(json, new JsonSerializerOptions().UseIronType());

            return "success";
        }
    }
}