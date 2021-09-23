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

            //var championsDtos = _mapper.Map<List<ChampionDto>>(champions);
            var championsDtos = new List<ChampionDto>();
            foreach (var champion in champions)
                championsDtos.Add(new() { Champion = champion, ChampionPassive = null, ChampionSpells = null });

            var result = new PageResult<ChampionDto>(championsDtos, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
        }

        public ChampionDto GetByName(string name)
        {
            var champion = _dbContex.Champions.FirstOrDefault(r => r.Name == name); // tutaj jakieœ inlcude zeby wszystko w jednym braæ
            if (champion is null) throw new NotFoundException("Champion not found"); 
            
            var championSpells = _dbContex.Champions_Spells.Where(r => r.Id.Contains(name));
            var championPassive = _dbContex.Champions_Passives.FirstOrDefault(r => r.Id.Contains(name));


            var result = new ChampionDto() { Champion = champion, ChampionPassive = championPassive, ChampionSpells = championSpells.ToList() }; // to jeszcze do przerobienia (ca³y model a potem mapowanie)
            //var result = _mapper.Map<RestaurantDto>(champion); //zmapujemy do ChampionDto z spellami itp

            return result;
        }
    }
}
