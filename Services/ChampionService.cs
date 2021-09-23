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

namespace WildRiftWebAPI
{
    public interface IChampionService
    {
        ChampionDto GetByName(string name);
        PageResult<ChampionDto> GetAll(ChampionQuery query);
    }

    public class ChampionService : IChampionService
    {
        private readonly ChampionDbContext _dbContex;
        private readonly IMapper _mapper;
        private readonly ILogger<ChampionService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public ChampionService(ChampionDbContext dbContex, IMapper mapper, ILogger<ChampionService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContex = dbContex;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public PageResult<ChampionDto> GetAll(ChampionQuery query)
        {
            var baseQuery = _dbContex.Champions
                .Include(r => r.ChampionSpells)
                .Include(r => r.ChampionPassive)
                .Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower()) || r.Title.ToLower().Contains(query.SearchPhrase.ToLower())));

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Champion, object>>>
                {
                    { nameof(Champion.Name), r => r.Name },
                    { nameof(Champion.Title), r => r.Title },
                };

                var selectedColumn = columnsSelector[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var champions = baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            int totalItemsCount = baseQuery.Count();

            var championsDtos = _mapper.Map<List<ChampionDto>>(champions);

            foreach (var championDto in championsDtos)
                championDto.ChampionSpells = championDto.ChampionSpells.OrderBy(ch => "QWER".IndexOf(ch.Id.Last())).ToList();

            var result = new PageResult<ChampionDto>(championsDtos, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
        }

        public ChampionDto GetByName(string name)
        {
            var champion = _dbContex.Champions.Include(r => r.ChampionSpells).Include(r => r.ChampionPassive).FirstOrDefault(r => r.Name == name); 

            if (champion is null) throw new NotFoundException("Champion not found");

            champion.ChampionSpells = champion.ChampionSpells.OrderBy(ch => "QWER".IndexOf(ch.Id.Last())).ToList();
            var result = _mapper.Map<ChampionDto>(champion);
            return result;
        }
    }
}
