using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WildRiftWebAPI
{
    public interface IItemService
    {
        ItemDto GetByName(string name);

        PageResult<ItemDto> GetAll(ItemQuery query);

        void Delete(string name);

        void Create(CreateItemDto dto);

        void Update(string name, UpdateItemDto updateItem);

        string GetProperty(string name, string property);
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

        public ItemDto GetByName(string name)
        {
            string approximatedName = Helpers.ApproximateName(name, _context.Items);

            if (approximatedName is "")
                throw new NotFoundException("Item not found");

            var items = _context.Items
                .AsNoTracking()
                .Where(item => item.Name.Contains(name) || item.Name.Contains(approximatedName));

            var item = items.FirstOrDefault(item => item.Name.Contains(name)) is not null 
                ? items.FirstOrDefault(r => r.Name.Contains(name)) 
                : items.FirstOrDefault(r => r.Name.Contains(approximatedName));

            var result = _mapper.Map<ItemDto>(item);
            return result;
        }

        public PageResult<ItemDto> GetAll(ItemQuery query)
        {
            var baseQuery = _context.Items
                .AsNoTracking()
                .Where(item => query.SearchPhrase == null || (item.Name.ToLower().Contains(query.SearchPhrase.ToLower())));

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Item, object>>>
                {
                    { nameof(Item.Name), item => item.Name },
                };

                var selectedColumn = columnsSelector[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var items = baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)          
                .ToList();

            int totalItemsCount = baseQuery.Count();

            var itemDtos = _mapper.Map<List<ItemDto>>(items);

            var result = new PageResult<ItemDto>(itemDtos, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
        }

        public void Delete(string name)
        {
            var item = _context.Items.FirstOrDefault(item => item.Name == name);

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
            var item = _context.Items.FirstOrDefault(item => item.Name == name);

            if (item is null)
                throw new NotFoundException("Item not found");

            foreach (var property in updateItem.GetType().GetProperties())
                if (property.GetValue(updateItem) is not null)
                    typeof(Item).GetProperty(property.Name).SetValue(item, property.GetValue(updateItem));

            _context.SaveChanges();
        }

        public string GetProperty(string name, string property)
        {
            var item = _context.Items.FirstOrDefault(item => item.Name == name);

            if (item is null)
                throw new NotFoundException("Item not found");

            Helpers.Capitalize(ref property);
            var foundProperty = typeof(Item).GetProperty(property);

            if (foundProperty is null)
                throw new NotFoundException("Statistic not found");

            var result = foundProperty.GetValue(item).ToString();
            return result;
        }
    }
}