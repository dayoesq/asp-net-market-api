using Market.DataContext;
using Market.Models;
using Market.Repository;

namespace Market.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<Product> Products { get; }
    IRepository<Category> Categories { get; }
    IRepository<Color> Colors { get; }
    IRepository<Size> Sizes { get; }
    void Save();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Products = new Repository<Product>(_context);
        Categories = new Repository<Category>(_context);
        Colors = new Repository<Color>(_context);
        Sizes = new Repository<Size>(_context);
        
    }

    public IRepository<Product> Products { get; }
    public IRepository<Category> Categories { get; }
    public IRepository<Color> Colors { get; }
    public IRepository<Size> Sizes { get; }

    public void Save()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
