using Core.Domain;

namespace Core.Data.Repositories;

public interface IPersonRepository
{
    Task AddAsync(Person person);
}

public class PersonRepository : IPersonRepository
{
    private readonly DatabaseContext _context;

    public PersonRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Person person)
    {
        _context.People.Add(person);
        await _context.SaveChangesAsync();
    }
}
