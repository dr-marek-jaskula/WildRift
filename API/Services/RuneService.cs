using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog.Web;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using Google.Protobuf.WellKnownTypes;
using System.Xml.Linq;

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
        private readonly WildRiftDbContext _dbContex;
        private readonly IMapper _mapper;
        private readonly ILogger<RuneService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public RuneService(WildRiftDbContext dbContex, IMapper mapper, ILogger<RuneService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContex = dbContex;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public RuneDto GetByName(string name)
        {
            string approximatedName = Helpers.ApproximateName(name, _dbContex.Runes);

            if (approximatedName is "")
                throw new NotFoundException("Rune not found");

            var runes = _dbContex.Runes.Where(r => r.Name.Contains(name) || r.Name.Contains(approximatedName));

            var rune = runes.FirstOrDefault(r => r.Name.Contains(name)) is not null ? runes.FirstOrDefault(r => r.Name.Contains(name)) : runes.FirstOrDefault(r => r.Name.Contains(approximatedName));

            var result = _mapper.Map<RuneDto>(rune);
            return result;
        }

        public PageResult<RuneDto> GetAll(RuneQuery query)
        {
            var baseQuery = _dbContex.Runes.Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())));

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
            _logger.LogWarning($"Rune with name: {name}. Delete action invoked");

            var rune = _dbContex.Runes.FirstOrDefault(rune => rune.Name == name);

            if (rune is null)
                throw new NotFoundException("Rune not found");

            _dbContex.Runes.Remove(rune);
            _dbContex.SaveChanges();
        }
        
        public void Create(CreateRuneDto createRune)
        {
            var rune = _mapper.Map<Rune>(createRune);

            _dbContex.Runes.Add(rune);
            _dbContex.SaveChanges();
        }
        
        public void Update(string name, UpdateRuneDto updateRune)
        {
            var rune = _dbContex.Runes.FirstOrDefault(r => r.Name == name);

            if (rune is null) 
                throw new NotFoundException("Rune not found");

            foreach (var property in updateRune.GetType().GetProperties())
                if (property.GetValue(updateRune) is not null)
                    typeof(Rune).GetProperty(property.Name).SetValue(rune, property.GetValue(updateRune));

            _dbContex.SaveChanges();
        }

        public string GetProperty(string name, string property)
        {
            var rune = _dbContex.Runes.FirstOrDefault(r => r.Name == name);

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