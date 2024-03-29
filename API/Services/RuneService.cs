using System.Linq.Expressions;
namespace WildRiftWebAPI;

public interface IRuneService
{
    Task<RuneDto> GetByName(string name);

    Task<PageResult<RuneDto>> GetAll(RuneQuery query);

    void Delete(string name);

    void Create(CreateRuneDto dto);

    void Update(string name, UpdateRuneDto updateRune);

    Task<string> GetProperty(string name, string property);
}

public class RuneService : IRuneService
{
    private readonly WildRiftDbContext _context;
    private readonly IMapper _mapper;

    public RuneService(WildRiftDbContext dbContex, IMapper mapper)
    {
        _context = dbContex;
        _mapper = mapper;
    }

    public async Task<RuneDto> GetByName(string name)
    {
        string approximatedName = Helpers.ApproximateName(name, _context.Runes);

        return await ((AsyncPolicy)PollyRegister.registry["AsyncCacheStrategy"]).ExecuteAsync(async context =>
        {
            if (approximatedName is "")
                throw new NotFoundException("Rune not found");

            var runes = _context.Runes
                .AsNoTracking()
                .Where(r => r.Name.Contains(name) || r.Name.Contains(approximatedName));

            var rune = await runes.FirstOrDefaultAsync(r => r.Name.Contains(name)) is not null
                ? await runes.FirstOrDefaultAsync(r => r.Name.Contains(name))
                : await runes.FirstOrDefaultAsync(r => r.Name.Contains(approximatedName));

            var result = _mapper.Map<RuneDto>(rune);
            return result;
        }, new Context($"{approximatedName}"));
    }

    public async Task<PageResult<RuneDto>> GetAll(RuneQuery query)
    {
        return await ((AsyncPolicy)PollyRegister.registry["AsyncCacheStrategy"]).ExecuteAsync(async context =>
        {
            var baseQuery = _context.Runes
            .AsNoTracking()
            .Where(r => query.SearchPhrase == null || r.Name.ToLower().Contains(query.SearchPhrase.ToLower()));

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var columnsSelector = new Dictionary<string, Expression<Func<Rune, object>>>
            {
                { nameof(Rune.Name), r => r.Name },
            };

            var selectedColumn = columnsSelector[query.SortBy];

            baseQuery = query.SortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var runes = await baseQuery
            .Skip(query.PageSize * (query.PageNumber - 1))
            .Take(query.PageSize)
            .ToListAsync();

        int totalRunesCount = baseQuery.Count();

        var runeDtos = _mapper.Map<List<RuneDto>>(runes);
        var result = new PageResult<RuneDto>(runeDtos, totalRunesCount, query.PageSize, query.PageNumber);

        return result;
        }, new Context($"{query}"));
    }

    public void Delete(string name)
    {
        var rune = _context.Runes.FirstOrDefault(r => r.Name == name);

        if (rune is null)
            throw new NotFoundException("Rune not found");

        _context.Runes.Remove(rune);
        _context.SaveChanges();
    }

    public void Create(CreateRuneDto createRune)
    {
        var rune = _mapper.Map<Rune>(createRune);

        _context.Runes.Add(rune);
        _context.SaveChanges();
    }

    public void Update(string name, UpdateRuneDto updateRune)
    {
        var rune = _context.Runes.FirstOrDefault(r => r.Name == name);

        if (rune is null)
            throw new NotFoundException("Rune not found");

        foreach (var property in updateRune.GetType().GetProperties())
            if (property.GetValue(updateRune) is not null)
                typeof(Rune).GetProperty(property.Name).SetValue(rune, property.GetValue(updateRune));

        _context.SaveChanges();
    }

    public async Task<string> GetProperty(string name, string property)
    {
        return await ((AsyncPolicy)PollyRegister.registry["AsyncCacheStrategy"]).ExecuteAsync(async context =>
        {
            var rune = await _context.Runes.FirstOrDefaultAsync(r => r.Name == name);

            if (rune is null)
                throw new NotFoundException("Rune not found");

            Helpers.Capitalize(ref property);
            var foundProperty = typeof(Rune).GetProperty(property);

            if (foundProperty is null)
                throw new NotFoundException("Field not found");

            var result = foundProperty.GetValue(rune).ToString();
            return result;
        }, new Context($"{name}{property}"));
    }
}
