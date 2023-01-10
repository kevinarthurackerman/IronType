namespace IronType.Examples.BlazorWasm.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public OrdersController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<OrderViewModel> Get(LocalDate? from, LocalDate? to)
        {
            var orders = _dbContext.Orders
                .Where(x => from == null || x.OrderedOn >= from && to == null || x.OrderedOn <= to)
                .Select(x => new OrderViewModel
                {
                    Id = x.Id,
                    OrderedOn = x.OrderedOn,
                    CustomerName = x.CustomerName,
                    Height = x.Height,
                    Length = x.Length,
                    Width = x.Width,
                    Weight = x.Weight
                })
                .ToArray();

            return orders;
        }
    }
}