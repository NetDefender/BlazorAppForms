using BlazorAppForms.Server.Data;
using BlazorAppForms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Buffers;

namespace BlazorAppForms.Server.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class CustomerController : Controller
{
    private readonly BlazorFormsContext _context;
    private readonly ILogger<CustomerController> _logger;
    private readonly IWebHostEnvironment _env;

    public CustomerController(ILogger<CustomerController> logger, IDbContextFactory<BlazorFormsContext> contextFactory, IWebHostEnvironment env)
    {
        _context = contextFactory.CreateDbContext();
        _logger = logger;
        _env = env;
    }

    private void GenerateFiles()
    {
        using FileStream fs400 = new(Path.Combine(_env.ContentRootPath, "Files", "Blob400.docx"), FileMode.Create);
        using FileStream fs500 = new(Path.Combine(_env.ContentRootPath, "Files", "Blob500.docx"), FileMode.Create);
        using FileStream fs600 = new(Path.Combine(_env.ContentRootPath, "Files", "Blob600.docx"), FileMode.Create);

        byte[] buffer = ArrayPool<byte>.Shared.Rent(1024 * 1024);
        Array.Fill<byte>(buffer, 64);

        WriteFile(fs400, buffer, 400);
        WriteFile(fs500, buffer, 500);
        WriteFile(fs600, buffer, 600);

        ArrayPool<byte>.Shared.Return(buffer);
    }

    private void WriteFile(FileStream fs, byte[] buffer, int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            fs.Write(buffer, 0, buffer.Length);
        }
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

    [HttpPost("upload")]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    [RequestSizeLimit(long.MaxValue)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        using (FileStream fs = new FileStream(Path.Combine(_env.ContentRootPath, "Files", file.FileName), FileMode.Create))
        {
            await file.CopyToAsync(fs).ConfigureAwait(false);
        }
        return Ok();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _context.Dispose();
    }
}
