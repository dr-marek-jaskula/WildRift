using System.Linq.Expressions;
namespace WildRiftWebAPI;

public interface IItemService
{
    Task<ItemDto> GetByName(string name);

    Task<PageResult<ItemDto>> GetAll(ItemQuery query);

    void Delete(string name);

    void Create(CreateItemDto dto);

    void Update(string name, UpdateItemDto updateItem);

    Task<string> GetProperty(string name, string property);
}

public class ItemService : IItemService
{
    private readonly WildRiftDbContext _context;
    private readonly IMapper _mapper;

    public ItemService(WildRiftDbContext dbContex, IMapper mapper)
    {
        _context = dbContex;
        _mapper = mapper;
    }

    public async Task<ItemDto> GetByName(string name)
    {
        string approximatedName = Helpers.ApproximateName(name, _context.Items);

        return await ((AsyncPolicy)PollyRegister.registry["AsyncCacheStrategy"]).ExecuteAsync(async context => 
        {
            if (approximatedName is "")
                throw new NotFoundException("Item not found");

            var items = _context.Items
                .AsNoTracking()
                .Where(i => i.Name.Contains(name) || i.Name.Contains(approximatedName));

            var item = await items.FirstOrDefaultAsync(i => i.Name.Contains(name)) is not null
                ? await items.FirstOrDefaultAsync(i => i.Name.Contains(name))
                : await items.FirstOrDefaultAsync(i => i.Name.Contains(approximatedName));

            var result = _mapper.Map<ItemDto>(item);
            return result;
        }, new Context($"{approximatedName}"));
    }

    public async Task<PageResult<ItemDto>> GetAll(ItemQuery query)
    {
        return await ((AsyncPolicy)PollyRegister.registry["AsyncCacheStrategy"]).ExecuteAsync(async context =>
        {
            var baseQuery = _context.Items
            .AsNoTracking()
            .Where(i => query.SearchPhrase == null || i.Name.ToLower().Contains(query.SearchPhrase.ToLower()));

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var columnsSelector = new Dictionary<string, Expression<Func<Item, object>>>
            {
                { nameof(Item.Name), i => i.Name },
            };

            var selectedColumn = columnsSelector[query.SortBy];

            baseQuery = query.SortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var items = await baseQuery
            .AsNoTracking()
            .Skip(query.PageSize * (query.PageNumber - 1))
            .Take(query.PageSize)          
            .ToListAsync();

        int totalItemsCount = baseQuery.Count();

        var itemDtos = _mapper.Map<List<ItemDto>>(items);

        var result = new PageResult<ItemDto>(itemDtos, totalItemsCount, query.PageSize, query.PageNumber);

        return result;
        }, new Context($"{query}"));
    }

    public void Delete(string name)
    {
        var item = _context.Items.FirstOrDefault(i => i.Name == name);

        if (item is null)
            throw new NotFoundException("Item not found");

        _context.Items.Remove(item);
        _context.SaveChanges();
    }

    public void Create(CreateItemDto createItem)
    {
        var item = _mapper.Map<Item>(createItem);

        _context.Items.Add(item);
        _context.SaveChanges();
    }

    public void Update(string name, UpdateItemDto updateItem)
    {
        var item = _context.Items.FirstOrDefault(i => i.Name == name);

        if (item is null)
            throw new NotFoundException("Item not found");

        foreach (var property in updateItem.GetType().GetProperties())
            if (property.GetValue(updateItem) is not null)
                typeof(Item).GetProperty(property.Name).SetValue(item, property.GetValue(updateItem));

        _context.SaveChanges();
    }

    public async Task<string> GetProperty(string name, string property)
    {
        return await ((AsyncPolicy)PollyRegister.registry["AsyncCacheStrategy"]).ExecuteAsync(async context =>
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Name == name);

            if (item is null)
                throw new NotFoundException("Item not found");

            Helpers.Capitalize(ref property);
            var foundProperty = typeof(Item).GetProperty(property);

            if (foundProperty is null)
                throw new NotFoundException("Statistic not found");

            var result = foundProperty.GetValue(item).ToString();
            return result;
        }, new Context($"{name}{property}"));
    }
}
