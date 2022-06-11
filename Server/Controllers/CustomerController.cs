using BlazorAppForms.Server.Data;
using BlazorAppForms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppForms.Server.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class CustomerController : Controller
{
    private readonly BlazorFormsContext _context;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ILogger<CustomerController> logger, IDbContextFactory<BlazorFormsContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
        _logger = logger;
    }

    [HttpGet("{idCustomer:int}")]
    public CustomerTransport Get(int idCustomer)
    {
        return _context.Customer
            .Include(c => c.CustomerLocation)
            .AsSplitQuery()
            .AsNoTracking()
            .First(c => c.IdCustomer == idCustomer)
            .Adapt<CustomerTransport>();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _context.Dispose();
    }
}
