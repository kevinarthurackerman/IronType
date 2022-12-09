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
        public IEnumerable<OrderViewModel> Get()
        {
            return _dbContext.Orders
                .Select(x => new OrderViewModel
                {
                    Id = x.Id,
                    OrderedOn = x.OrderedOn,
                    CustomerName = x.CustomerName
                })
                .ToArray();
        }
    }
}