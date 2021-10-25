using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WildRiftWebAPI
{
    public interface IRuneService
    {
        RuneDto GetByName(string name);

        PageResult<RuneDto> GetAll(RuneQuery query);

        void Delete(string name);

        void Create(CreateRuneDto dto);

        void Update(string name, UpdateRuneDto updateRune);

        string GetProperty(string name, string property);
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

        public RuneDto GetByName(string name)
        {
            string approximatedName = Helpers.ApproximateName(name, _context.Runes);

            if (approximatedName is "")
                throw new NotFoundException("Rune not found");

            var runes = _context.Runes
                .AsNoTracking()
                .Where(r => r.Name.Contains(name) || r.Name.Contains(approximatedName));

            var rune = runes.FirstOrDefault(r => r.Name.Contains(name)) is not null 
                ? runes.FirstOrDefault(r => r.Name.Contains(name)) 
                : runes.FirstOrDefault(r => r.Name.Contains(approximatedName));

            var result = _mapper.Map<RuneDto>(rune);
            return result;
        }

        public PageResult<RuneDto> GetAll(RuneQuery query)
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

            var runes = baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            int totalRunesCount = baseQuery.Count();

            var runeDtos = _mapper.Map<List<RuneDto>>(runes);

            var result = new PageResult<RuneDto>(runeDtos, totalRunesCount, query.PageSize, query.PageNumber);

            return result;
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

        public string GetProperty(string name, string property)
        {
            var rune = _context.Runes.FirstOrDefault(r => r.Name == name);

            if (rune is null)
                throw new NotFoundException("Rune not found");

            Helpers.Capitalize(ref property);
            var foundProperty = typeof(Rune).GetProperty(property);

            if (foundProperty is null)
                throw new NotFoundException("Field not found");

            var result = foundProperty.GetValue(rune).ToString();
            return result;
        }
    }
}